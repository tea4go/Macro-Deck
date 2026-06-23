using System.Net.WebSockets;

namespace SuchByte.MacroDeck.DataTypes;

/// <summary>
/// WebSocket 关闭原因类，封装关闭状态码和描述信息。
/// 用于在关闭 WebSocket 连接时传递关闭原因。
/// </summary>
public class WebSocketCloseReason
{
    /// <summary>关闭状态码</summary>
    public WebSocketCloseStatus Status { get; }

    /// <summary>关闭原因描述</summary>
    public string Description { get; }

    /// <summary>
    /// 构造函数，指定关闭状态码和描述
    /// </summary>
    /// <param name="status">WebSocket 关闭状态码</param>
    /// <param name="description">关闭原因描述</param>
    public WebSocketCloseReason(WebSocketCloseStatus status, string description)
    {
        Status = status;
        Description = description;
    }
}
