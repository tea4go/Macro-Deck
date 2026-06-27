using System.IO;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using Serilog;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Server;

/// <summary>
/// ADB 服务器辅助类，管理 Android Debug Bridge 服务器的启动和设备连接。
/// 当 Android 设备通过 USB 连接时，自动启动 Macro Deck 客户端应用
/// 并设置反向端口转发，使设备可以通过 localhost 访问 Macro Deck 服务器。
/// </summary>
public class AdbServerHelper
{
    private static readonly ILogger Logger = Log.ForContext(typeof(AdbServerHelper));

    /// <summary>ADB 服务器实例</summary>
    private static AdbServer? _adbServer;

    /// <summary>ADB 文件夹名称</summary>
    private const string AdbFolderName = "Android Debug Bridge";

    /// <summary>ADB 可执行文件路径</summary>
    private static readonly string AdbPath = Path.Combine(ApplicationPaths.MainDirectoryPath, AdbFolderName, "adb.exe");

    /// <summary>
    /// 停止 ADB 服务器
    /// </summary>
    public static async Task Stop()
    {
        if (_adbServer is null)
        {
            return;
        }

        await _adbServer.StopServerAsync();
    }

    /// <summary>
    /// 初始化 ADB 服务器。如果配置中启用了 ADB 且 adb.exe 存在，
    /// 则启动 ADB 服务器并监听设备连接/断开事件。
    /// </summary>
    public static async Task Initialize()
    {
        if (!MacroDeck.Configuration.EnableAdbServer)
        {
            return;
        }

        if (!File.Exists(AdbPath))
        {
            Logger.Warning("无法在 {AdbPath} 启动 ADB 服务器：找不到文件", AdbPath);
            return;
        }

        Logger.Information("正在使用 {AdbPath} 启动 ADB 服务器", AdbPath);

        _adbServer = new AdbServer();
        var result = await _adbServer.StartServerAsync(AdbPath, false);
        if (result != StartServerResult.Started && result != StartServerResult.AlreadyRunning)
        {
            Logger.Information("无法启动 ADB 服务器");
        }

        // 启动设备监视器，监听设备连接和断开事件
        var monitor = new DeviceMonitor(new AdbSocket(AdbClient.AdbServerEndPoint));
        monitor.DeviceConnected += Monitor_DeviceConnected;
        monitor.DeviceDisconnected += Monitor_DeviceDisconnected;
        await monitor.StartAsync();
    }

    /// <summary>
    /// 在指定设备上执行异步操作。先获取 ADB 客户端和设备数据，然后执行操作。
    /// </summary>
    /// <param name="serial">设备序列号</param>
    /// <param name="action">要执行的异步操作</param>
    private static async Task RunForDevice(string serial, Func<AdbClient, DeviceData, Task> action)
    {
        var adbClient = await GetAdbClient();
        if (adbClient is null)
        {
            return;
        }

        var device = await GetDevice(adbClient, serial);
        if (device is null)
        {
            return;
        }

        await action(adbClient, device);
    }

    /// <summary>
    /// 根据序列号获取设备数据
    /// </summary>
    /// <param name="adbDeviceClient">ADB 客户端</param>
    /// <param name="serial">设备序列号</param>
    /// <returns>设备数据，未找到返回 null</returns>
    private static async Task<DeviceData?> GetDevice(AdbClient adbDeviceClient, string serial)
    {
        var devices = await adbDeviceClient.GetDevicesAsync();
        return devices.FirstOrDefault(x => x.Serial.Equals(serial));
    }

    /// <summary>
    /// 获取已连接的 ADB 客户端实例
    /// </summary>
    /// <returns>ADB 客户端，连接失败返回 null</returns>
    private static async Task<AdbClient?> GetAdbClient()
    {
        var serverEndpoint = GetAdbServerEndpoint();
        if (serverEndpoint is null)
        {
            return null;
        }

        var adbClient = new AdbClient();
        await adbClient.ConnectAsync(serverEndpoint);
        return adbClient;
    }

    /// <summary>
    /// 获取 ADB 服务器端点地址
    /// </summary>
    /// <returns>端点字符串，服务器未运行返回 null</returns>
    private static string? GetAdbServerEndpoint()
    {
        var adbServerEndpoint = _adbServer?.EndPoint.ToString();
        if (adbServerEndpoint is not null)
        {
            return adbServerEndpoint;
        }

        Logger.Information("ADB 服务器端点为空");
        return null;
    }

    /// <summary>
    /// 设备断开连接事件处理
    /// </summary>
    private static void Monitor_DeviceDisconnected(object sender, DeviceDataEventArgs e)
    {
        Logger.Information("设备 {DeviceName} 已断开连接", e.Device.Name);
    }

    /// <summary>
    /// 设备连接事件处理。排除模拟器设备（127.0.0.1 开头），
    /// 对真实 USB 设备启动 Macro Deck 客户端并设置反向端口转发。
    /// </summary>
    private static async void Monitor_DeviceConnected(object sender, DeviceDataEventArgs e)
    {
        // 排除模拟器设备
        if (e.Device.Serial.StartsWith("127.0.0.1"))
        {
            return;
        }

        Logger.Information("设备 {DeviceName} 已连接", e.Device.Name);
        await RunForDevice(e.Device.Serial,
            async (adbDeviceClient, deviceData) =>
            {
                await StartMacroDeckClient(adbDeviceClient, deviceData);
                await StartReverseForward(adbDeviceClient, deviceData);
            });
    }

    /// <summary>
    /// 等待设备上线。最多等待 20 秒，超时返回 false。
    /// </summary>
    /// <param name="device">目标设备</param>
    /// <returns>设备在线返回 true，超时返回 false</returns>
    private static async Task<bool> IsDeviceOnline(DeviceData device)
    {
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(20));

        await Task.WhenAny([timeoutTask, WaitForDeviceOnline()]);

        if (device.State == DeviceState.Online)
        {
            return true;
        }

        Logger.Information("设备 {DeviceSerial} 仍未上线 - {DeviceState}", device.Serial, device.State);
        return false;

        async Task WaitForDeviceOnline()
        {
            while (device.State != DeviceState.Online)
            {
                await Task.Delay(100);
            }
        }
    }

    /// <summary>
    /// 退出设备上的 Macro Deck 客户端应用并发送休眠按键
    /// </summary>
    /// <param name="adbDeviceClient">ADB 客户端</param>
    /// <param name="device">目标设备</param>
    private static async Task ExitMacroDeckClient(AdbClient adbDeviceClient, DeviceData device)
    {
        var deviceIsOnline = await IsDeviceOnline(device);
        if (!deviceIsOnline)
        {
            return;
        }

        try
        {
            await adbDeviceClient.ExecuteRemoteCommandAsync("am force-stop com.suchbyte.macrodeck",
                device,
                new ConsoleOutputReceiver());
            await adbDeviceClient.SendKeyEventAsync(device, "KEYCODE_SLEEP");
        }
        catch
        {
        }
    }

    /// <summary>
    /// 在设备上启动 Macro Deck 客户端应用。
    /// 先唤醒设备屏幕，然后启动应用主活动。
    /// 仅在配置中启用了 ADB 自动启动应用时执行。
    /// </summary>
    /// <param name="adbDeviceClient">ADB 客户端</param>
    /// <param name="device">目标设备</param>
    private static async Task StartMacroDeckClient(AdbClient adbDeviceClient, DeviceData device)
    {
        if (!MacroDeck.Configuration.EnableAdbAutoStartApp)
        {
            return;
        }

        var deviceIsOnline = await IsDeviceOnline(device);
        if (!deviceIsOnline)
        {
            return;
        }

        try
        {
            // 唤醒设备屏幕
            await adbDeviceClient.SendKeyEventAsync(device, "KEYCODE_WAKEUP");
            // 启动 Macro Deck 应用
            await adbDeviceClient.ExecuteRemoteCommandAsync("am start -n com.suchbyte.macrodeck/.MainActivity",
                device,
                new ConsoleOutputReceiver());
        }
        catch
        {
        }
    }

    /// <summary>
    /// 在设备上设置反向端口转发。
    /// 使设备可以通过 localhost:port 访问电脑上的 Macro Deck 服务器，
    /// 无需知道电脑的实际 IP 地址。
    /// </summary>
    /// <param name="adbDeviceClient">ADB 客户端</param>
    /// <param name="device">目标设备</param>
    private static async Task StartReverseForward(AdbClient adbDeviceClient, DeviceData device)
    {
        var deviceIsOnline = await IsDeviceOnline(device);
        if (!deviceIsOnline)
        {
            return;
        }

        try
        {
            await adbDeviceClient.CreateReverseForwardAsync(device,
                $"tcp:{MacroDeck.Configuration.HostPort}",
                $"tcp:{MacroDeck.Configuration.HostPort}",
                true);

            Logger.Information("已在设备 {DeviceName} 上启用反向端口转发", device.Name);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "无法在设备 {DeviceName} 上启用反向端口转发", device.Name);
        }
    }
}
