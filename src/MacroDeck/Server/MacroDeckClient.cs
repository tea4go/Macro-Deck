using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Folders;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Server.DeviceMessage;

namespace SuchByte.MacroDeck.Server;

/// <summary>
/// Macro Deck 客户端类，表示一个已连接的远程客户端设备。
/// 包含客户端的会话 ID、设备 ID、设备类型、配置文件、当前文件夹和设备消息处理器。
/// </summary>
public class MacroDeckClient
{
    /// <summary>
    /// 构造函数，初始化客户端的会话 ID
    /// </summary>
    /// <param name="sessionId">WebSocket 会话 ID</param>
    public MacroDeckClient(string sessionId)
    {
        SessionId = sessionId;
    }

    /// <summary>设备类型字段</summary>
    private DeviceType _deviceType;

    /// <summary>
    /// 设置客户端的设备 ID
    /// </summary>
    /// <param name="clientId">设备 ID</param>
    public void SetClientId(string clientId)
    {
        ClientId = clientId;
    }

    /// <summary>客户端当前显示的文件夹</summary>
    public MacroDeckFolder Folder { get; set; }

    /// <summary>客户端分配的配置文件</summary>
    public MacroDeckProfile Profile { get; set; }

    /// <summary>客户端设备 ID（由客户端提供）</summary>
    public string ClientId { get; private set; }

    /// <summary>WebSocket 会话 ID</summary>
    public string SessionId { get; private set; }

    /// <summary>设备分类，默认为软件客户端</summary>
    public DeviceClass DeviceClass { get; set; } = DeviceClass.SoftwareClient;

    /// <summary>
    /// 设备类型。设置时根据设备类型自动选择对应的消息处理器。
    /// 目前所有设备类型都使用 SoftwareClientMessage。
    /// </summary>
    public DeviceType DeviceType
    {
        get => _deviceType;
        set
        {
            _deviceType = value;
            switch (_deviceType)
            {
                case DeviceType.Unknown:
                case DeviceType.Web:
                case DeviceType.Android:
                case DeviceType.StreamDeck:
                case DeviceType.iOS:
                    DeviceClass = DeviceClass.SoftwareClient;
                    DeviceMessage = new SoftwareClientMessage();
                    break;
            }
        }
    }

    /// <summary>设备消息处理器，用于向客户端发送各种消息</summary>
    public IDeviceMessage DeviceMessage { get; set; }
}
