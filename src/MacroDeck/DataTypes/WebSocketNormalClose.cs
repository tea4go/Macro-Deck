using System.Net.WebSockets;

namespace SuchByte.MacroDeck.DataTypes;

/// <summary>
/// WebSocket 正常关闭原因，使用标准 NormalClosure 状态码和 "Closing" 描述。
/// 用于客户端或服务端主动正常断开连接的场景。
/// </summary>
public class WebSocketNormalClose : WebSocketCloseReason
{
    /// <summary>
    /// 构造函数，初始化为正常关闭状态
    /// </summary>
    public WebSocketNormalClose()
        : base(WebSocketCloseStatus.NormalClosure, "Closing")
    {
    }
}
