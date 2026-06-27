using System.IO;
using System.Media;
using Newtonsoft.Json;
using Serilog;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Device;

/// <summary>
/// 设备管理器，负责管理所有已知的 Macro Deck 客户端设备。
/// 提供设备的加载、保存、添加、删除、重命名、配置文件分配、
/// 连接请求处理等功能。设备信息以 JSON 格式持久化存储。
/// </summary>
public class DeviceManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(DeviceManager));

    /// <summary>
    /// 已知设备列表
    /// </summary>
    private static List<MacroDeckDevice> _macroDeckDevices = new();

    /// <summary>
    /// 保存设备列表时的文件锁，防止并发写入
    /// </summary>
    private static readonly object _saveLock = new();

    /// <summary>
    /// 设备列表变化事件
    /// </summary>
    public static event EventHandler OnDevicesChange;

    /// <summary>
    /// 从文件加载已知设备列表。如果文件损坏则删除并重置。
    /// </summary>
    public static void LoadKnownDevices()
    {
        if (!File.Exists(ApplicationPaths.DevicesFilePath))
        {
            return;
        }

        try
        {
            _macroDeckDevices = JsonConvert.DeserializeObject<List<MacroDeckDevice>>(
                File.ReadAllText(ApplicationPaths.DevicesFilePath),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                })!;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "devices.json 已损坏，将重置");
            try
            {
                File.Delete(ApplicationPaths.DevicesFilePath);
            }
            catch
            {
            }

            _macroDeckDevices = new List<MacroDeckDevice>();
        }
    }

    /// <summary>
    /// 将已知设备列表保存到文件。使用临时文件和原子替换确保写入安全。
    /// </summary>
    public static void SaveKnownDevices()
    {
        var serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        try
        {
            // 先写入临时文件，再原子替换，避免写入中断导致数据丢失
            var tempPath = ApplicationPaths.DevicesFilePath + ".tmp";
            lock (_saveLock)
            {
                using (var sw = new StreamWriter(tempPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, _macroDeckDevices);
                }

                File.Move(tempPath, ApplicationPaths.DevicesFilePath, true);
            }

            OnDevicesChange?.Invoke(null, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "保存已知设备列表时发生错误");
        }
    }

    /// <summary>
    /// 添加已知设备（MacroDeckDevice 类型）。如果设备已存在则不重复添加。
    /// </summary>
    /// <param name="macroDeckDevice">要添加的设备</param>
    public static void AddKnownDevice(MacroDeckDevice macroDeckDevice)
    {
        if (!_macroDeckDevices.Contains(macroDeckDevice) &&
            _macroDeckDevices.Find(x => x.ClientId.Equals(macroDeckDevice.ClientId)) == null)
        {
            _macroDeckDevices.Add(macroDeckDevice);
        }

        SaveKnownDevices();
    }

    /// <summary>
    /// 检查指定客户端 ID 的设备是否为已知设备
    /// </summary>
    /// <param name="clientId">客户端唯一标识</param>
    /// <returns>如果是已知设备返回 true</returns>
    public static bool IsKnownDevice(string clientId)
    {
        foreach (var macroDeckDevice in _macroDeckDevices)
        {
            if (macroDeckDevice.ClientId.Equals(clientId))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 根据客户端 ID 获取设备信息
    /// </summary>
    /// <param name="clientId">客户端唯一标识</param>
    /// <returns>设备实例，未找到则返回 null</returns>
    public static MacroDeckDevice? GetMacroDeckDevice(string clientId)
    {
        return _macroDeckDevices.FirstOrDefault(macroDeckDevice => macroDeckDevice.ClientId.Equals(clientId));
    }

    /// <summary>
    /// 根据显示名称获取设备信息
    /// </summary>
    /// <param name="displayName">设备显示名称</param>
    /// <returns>设备实例，未找到则返回 null</returns>
    public static MacroDeckDevice? GetMacroDeckDeviceByDisplayName(string displayName)
    {
        return _macroDeckDevices.FirstOrDefault(macroDeckDevice => macroDeckDevice.DisplayName.Equals(displayName));
    }

    /// <summary>
    /// 为设备分配配置文件，同时更新本地存储和在线设备
    /// </summary>
    /// <param name="macroDeckDevice">目标设备</param>
    /// <param name="macroDeckProfile">要分配的配置文件</param>
    public static void SetProfile(MacroDeckDevice macroDeckDevice, MacroDeckProfile macroDeckProfile)
    {
        if (_macroDeckDevices.Contains(macroDeckDevice))
        {
            macroDeckDevice.ProfileId = macroDeckProfile.ProfileId;
            SaveKnownDevices();
        }

        // 如果设备在线，立即推送配置文件变更
        if (macroDeckDevice.Available)
        {
            MacroDeckServer.SetProfile(MacroDeckServer.GetMacroDeckClient(macroDeckDevice.ClientId), macroDeckProfile);
        }
    }

    /// <summary>
    /// 设置设备的阻止状态。被阻止的设备将立即断开连接。
    /// </summary>
    /// <param name="macroDeckDevice">目标设备</param>
    /// <param name="blocked">是否阻止</param>
    public static void SetBlocked(MacroDeckDevice macroDeckDevice, bool blocked)
    {
        if (_macroDeckDevices.Contains(macroDeckDevice))
        {
            macroDeckDevice.Blocked = blocked;
            SaveKnownDevices();
        }

        // 如果设置为阻止且设备在线，立即断开其连接
        if (!blocked || !macroDeckDevice.Available)
        {
            return;
        }

        var macroDeckClient = MacroDeckServer.GetMacroDeckClient(macroDeckDevice.ClientId);
        if (macroDeckClient is not null)
        {
            Task.Run(async () => await WebSocketHandler.Close(macroDeckClient.SessionId));
        }
    }

    /// <summary>
    /// 重命名设备的显示名称
    /// </summary>
    /// <param name="macroDeckDevice">目标设备</param>
    /// <param name="displayName">新的显示名称</param>
    public static void RenameMacroDeckDevice(MacroDeckDevice macroDeckDevice, string displayName)
    {
        if (!_macroDeckDevices.Contains(macroDeckDevice))
        {
            return;
        }

        macroDeckDevice.DisplayName = displayName;
        SaveKnownDevices();
    }

    /// <summary>
    /// 从已知设备列表中移除指定设备
    /// </summary>
    /// <param name="macroDeckDevice">要移除的设备</param>
    public static void RemoveKnownDevice(MacroDeckDevice macroDeckDevice)
    {
        if (!_macroDeckDevices.Contains(macroDeckDevice))
        {
            return;
        }

        _macroDeckDevices.Remove(macroDeckDevice);
        SaveKnownDevices();
    }

    /// <summary>
    /// 检查显示名称是否可用（未被其他设备使用）
    /// </summary>
    /// <param name="displayName">要检查的显示名称</param>
    /// <returns>名称可用返回 true</returns>
    public static bool IsDisplayNameAvailable(string displayName)
    {
        return !(_macroDeckDevices.FindAll(macroDeckDevice => macroDeckDevice.DisplayName.Equals(displayName)).Count >
            0);
    }

    /// <summary>
    /// 获取所有已知设备列表
    /// </summary>
    /// <returns>已知设备列表</returns>
    public static List<MacroDeckDevice> GetKnownDevices()
    {
        return _macroDeckDevices;
    }

    /// <summary>
    /// 处理设备连接请求。根据配置决定是否需要用户确认。
    /// 已知且未被阻止的设备直接允许连接，新设备根据配置弹出确认对话框。
    /// </summary>
    /// <param name="macroDeckClient">请求连接的客户端信息</param>
    /// <returns>允许连接返回 true，拒绝返回 false</returns>
    public static bool RequestConnection(MacroDeckClient macroDeckClient)
    {
        if (MacroDeck.Configuration.AskOnNewConnections)
        {
            // 配置要求新连接需要确认
            if (IsKnownDevice(macroDeckClient.ClientId))
            {
                // 已知设备：检查是否被阻止
                var macroDeckDevice = GetMacroDeckDevice(macroDeckClient.ClientId);
                if (macroDeckDevice is { Blocked: true })
                {
                    return false;
                }

                // 更新设备信息
                macroDeckDevice!.ClientId = macroDeckClient.ClientId;
                macroDeckDevice.DeviceType = macroDeckClient.DeviceType;
                return true;
            }

            // 未知设备：弹出确认对话框
            Form? mainForm = null;
            var dialogResult = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("MainWindow"))
                {
                    mainForm = form;
                }
            }

            // 确保在 UI 线程上显示对话框
            if (mainForm is { IsHandleCreated: true, IsDisposed: false })
            {
                mainForm.Invoke(() => { dialogResult = ShowConnectionDialog(macroDeckClient); });
            }
            else
            {
                dialogResult = ShowConnectionDialog(macroDeckClient);
            }

            if (!dialogResult)
            {
                return dialogResult;
            }

            {
                AddKnownDevice(macroDeckClient);
            }
            return dialogResult;
        }

        // 配置不要求确认：自动添加新设备
        if (!IsKnownDevice(macroDeckClient.ClientId))
        {
            AddKnownDevice(macroDeckClient);
        }

        return true;
    }

    /// <summary>
    /// 从客户端信息创建并添加新的已知设备
    /// </summary>
    /// <param name="macroDeckClient">客户端连接信息</param>
    public static void AddKnownDevice(MacroDeckClient macroDeckClient)
    {
        var macroDeckDevice = new MacroDeckDevice
        {
            ClientId = macroDeckClient.ClientId,
            DisplayName = "Client " + macroDeckClient.ClientId,
            ProfileId = ProfileManager.Profiles.FirstOrDefault()?.ProfileId ?? "0"
        };
        AddKnownDevice(macroDeckDevice);
        macroDeckDevice.ClientId = macroDeckClient.ClientId;
        macroDeckDevice.DeviceType = macroDeckClient.DeviceType;
    }

    /// <summary>
    /// 显示新连接确认对话框，播放提示音并等待用户决定。
    /// 用户可以选择允许、拒绝或阻止该设备。
    /// </summary>
    /// <param name="macroDeckClient">请求连接的客户端信息</param>
    /// <returns>允许连接返回 true，拒绝返回 false</returns>
    private static bool ShowConnectionDialog(MacroDeckClient macroDeckClient)
    {
        // 播放系统提示音
        SystemSounds.Exclamation.Play();
        using var newConnectionDialog = new NewConnectionDialog(macroDeckClient);
        if (newConnectionDialog.ShowDialog() == DialogResult.Yes)
        {
            return true;
        }

        // 用户拒绝连接，关闭该客户端的 WebSocket 连接
        Task.Run(async () => await WebSocketHandler.Close(macroDeckClient.SessionId));

        // 如果用户选择阻止该设备，将其添加到已知设备列表并标记为阻止
        if (newConnectionDialog.Blocked)
        {
            var macroDeckDevice = new MacroDeckDevice
            {
                ClientId = macroDeckClient?.ClientId,
                DisplayName = "Client " + macroDeckClient.ClientId,
                Blocked = true
            };
            AddKnownDevice(macroDeckDevice);
        }

        return false;
    }
}
