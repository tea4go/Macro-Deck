using System.Net.NetworkInformation;
using System.Net.Sockets;
using Serilog;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 网络工具类，提供本机网络接口的 IPv4 地址查询功能。
/// 用于配置界面中显示可用的网络适配器 IP 地址。
/// </summary>
internal class NetworkUtils
{
    private static readonly ILogger Logger = Log.ForContext(typeof(NetworkUtils));

    /// <summary>
    /// 获取所有活动网络接口的 IPv4 地址列表。
    /// 过滤规则与初始向导中的网卡选择口径保持一致：
    /// 仅保留 OperationalStatus=Up、非回环、且具备 IPv4 地址的接口。
    /// </summary>
    /// <returns>IPv4 地址字符串数组</returns>
    public static string[] GetNetworkInterfaces()
    {
        var networkInterfaces = new List<string>();
        try
        {
            networkInterfaces.AddRange(NetworkInterface.GetAllNetworkInterfaces()
                .Where(adapter => adapter.OperationalStatus == OperationalStatus.Up
                                  && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(adapter => adapter.GetIPProperties()
                    .UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
                    ?.Address.ToString())
                .Where(address => !string.IsNullOrWhiteSpace(address)));
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "搜索网络接口时发生错误");
        }

        return networkInterfaces.ToArray();
    }
}
