using System.Net.Sockets;
using System.Timers;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace SuchByte.MacroDeck.Server;

/// <summary>
/// UDP 广播服务器，定期向局域网广播 Macro Deck 服务器的连接信息。
/// 客户端通过接收广播消息自动发现局域网内的 Macro Deck 服务器。
/// 每 5 秒广播一次，包含计算机名、IP 地址和端口号。
/// </summary>
public static class BroadcastServer
{
    /// <summary>UDP 客户端，用于发送广播数据报</summary>
    private static UdpClient _udpClient;

    /// <summary>广播定时器，每 5 秒触发一次</summary>
    private static Timer _broadcastTimer;

    /// <summary>
    /// 启动广播服务器。创建 UDP 客户端和定时器，开始定期广播。
    /// </summary>
    public static void Start()
    {
        try
        {
            _udpClient = new UdpClient();

            _broadcastTimer = new Timer(1000 * 5)
            {
                Enabled = true
            };
            _broadcastTimer.Elapsed += BroadcastTimer_Elapsed;

            Application.ApplicationExit += OnApplicationExit;
        }
        catch
        {
        }
    }

    /// <summary>
    /// 应用程序退出时停止广播服务器
    /// </summary>
    private static void OnApplicationExit(object sender, EventArgs e)
    {
        Application.ApplicationExit -= OnApplicationExit;
        Stop();
    }

    /// <summary>
    /// 停止广播服务器，释放定时器和 UDP 客户端资源
    /// </summary>
    public static void Stop()
    {
        try
        {
            if (_broadcastTimer != null)
            {
                _broadcastTimer.Stop();
                _broadcastTimer.Elapsed -= BroadcastTimer_Elapsed;
                _broadcastTimer.Dispose();
                _broadcastTimer = null;
            }

            _udpClient?.Dispose();
            _udpClient = null;
        }
        catch
        {
        }
    }

    /// <summary>
    /// 定时器触发回调，向广播地址 255.255.255.255 发送服务器信息。
    /// 广播内容包含计算机名、IP 地址和端口号。
    /// </summary>
    private static void BroadcastTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        try
        {
            Task.Run(() =>
            {
                var broacastObject = new JObject
                {
                    ["computer-name"] = Environment.MachineName,
                    ["ip-address"] = MacroDeck.Configuration.HostAddress,
                    ["port"] = MacroDeck.Configuration.HostPort
                };
                var data = Encoding.UTF8.GetBytes(broacastObject.ToString());
                _udpClient.Send(data, data.Length, "255.255.255.255", MacroDeck.Configuration.HostPort);
            });
        }
        catch
        {
        }
    }
}
