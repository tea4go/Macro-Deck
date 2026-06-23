using Newtonsoft.Json;
using SuchByte.MacroDeck.Server;

namespace SuchByte.MacroDeck.Device;

/// <summary>
/// Macro Deck 设备模型，表示一个已连接或已知的外部客户端设备。
/// 包含设备的标识、显示名称、在线状态、阻止标志、配置文件关联和设备配置等信息。
/// </summary>
public class MacroDeckDevice
{
    /// <summary>客户端唯一标识</summary>
    public string ClientId { get; set; }

    /// <summary>设备显示名称</summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 设备是否在线（当前已连接到服务器）。
    /// 通过检查 WebSocket 连接状态实时判断。
    /// </summary>
    [JsonIgnore]
    public bool Available
    {
        get
        {
            var macroDeckClient = MacroDeckServer.GetMacroDeckClient(ClientId);
            if (macroDeckClient != null && WebSocketHandler.IsAvailable(macroDeckClient.SessionId))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>是否被阻止连接</summary>
    public bool Blocked { get; set; } = false;

    /// <summary>分配给此设备的配置文件 ID</summary>
    public string? ProfileId { get; set; }

    /// <summary>设备特定配置（亮度、自动连接、唤醒锁定等）</summary>
    public DeviceConfiguration Configuration { get; set; } = new();

    /// <summary>设备类型</summary>
    public DeviceType DeviceType { get; set; }
}
