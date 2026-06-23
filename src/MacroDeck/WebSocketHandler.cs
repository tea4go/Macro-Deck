using System.Net.WebSockets;
using SuchByte.MacroDeck.DataTypes;

namespace SuchByte.MacroDeck;

/// <summary>
/// WebSocket 连接处理器，管理所有 WebSocket 会话的生命周期和消息路由。
/// 负责会话的创建、消息收发、连接状态通知和会话清理。
/// </summary>
public class WebSocketHandler
{
    /// <summary>消息接收事件，参数为消息内容</summary>
    public static event EventHandler<string>? MessageReceived;

    /// <summary>会话连接成功事件</summary>
    public static event EventHandler? SessionConnected;

    /// <summary>会话断开事件</summary>
    public static event EventHandler? SessionDisconnected;

    /// <summary>所有活跃的客户端 WebSocket 会话列表</summary>
    private static List<WebSocketSession> ClientSessions { get; } = new();

    /// <summary>
    /// 向所有已连接的客户端广播消息
    /// </summary>
    /// <param name="message">要发送的消息</param>
    public static async Task SendMessageToAll(string message)
    {
        await SendMessageToMany(ClientSessions, message);
    }

    /// <summary>
    /// 向多个指定会话发送消息（并行发送）
    /// </summary>
    /// <param name="webSocketSessions">目标会话列表</param>
    /// <param name="message">要发送的消息</param>
    public static async Task SendMessageToMany(IList<WebSocketSession> webSocketSessions, string message)
    {
        var tasks = webSocketSessions.Select(session => session.SendMessage(message))
            .ToList();
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 向指定 ID 的客户端发送消息
    /// </summary>
    /// <param name="id">目标客户端会话 ID</param>
    /// <param name="message">要发送的消息</param>
    public static async Task SendMessageToClient(string id, string message)
    {
        var session = ClientSessions.FirstOrDefault(x => x.Id.Equals(id));
        if (session == null)
        {
            return;
        }

        await session.SendMessage(message);
    }

    /// <summary>
    /// 处理新的 WebSocket 连接。创建会话对象，注册事件处理器，
    /// 添加到会话列表并触发连接事件，然后启动会话消息循环。
    /// </summary>
    /// <param name="webSocket">WebSocket 连接实例</param>
    internal static async Task HandleWebSocket(WebSocket webSocket)
    {
        var session = new WebSocketSession(webSocket);
        session.TextMessageReceived += SessionOnTextMessageReceived;
        session.Disconnected += SessionOnDisconnected;

        // 线程安全地添加会话到列表
        lock (ClientSessions)
        {
            ClientSessions.Add(session);
        }

        SessionConnected?.Invoke(session, EventArgs.Empty);
        await session.Start();
    }

    /// <summary>
    /// 会话断开事件处理。触发断开事件，取消事件订阅，释放会话资源并从列表移除。
    /// </summary>
    private static void SessionOnDisconnected(object? sender, EventArgs e)
    {
        if (sender is not WebSocketSession session)
        {
            return;
        }

        SessionDisconnected?.Invoke(session, EventArgs.Empty);

        // 取消事件订阅并释放资源
        session.TextMessageReceived -= SessionOnTextMessageReceived;
        session.Disconnected -= SessionOnDisconnected;
        session.Dispose();
        ClientSessions.Remove(session);
    }

    /// <summary>
    /// 会话文本消息接收事件处理，转发为 MessageReceived 事件
    /// </summary>
    private static void SessionOnTextMessageReceived(object? sender, string message)
    {
        if (sender is not WebSocketSession session)
        {
            return;
        }

        MessageReceived?.Invoke(session, message);
    }

    /// <summary>
    /// 关闭指定 ID 的客户端会话
    /// </summary>
    /// <param name="sessionId">要关闭的会话 ID</param>
    public static async Task Close(string sessionId)
    {
        var session = ClientSessions.FirstOrDefault(x => x.Id == sessionId);
        if (session is null)
        {
            return;
        }

        await session.Close();
    }

    /// <summary>
    /// 检查指定 ID 的会话是否仍然活跃
    /// </summary>
    /// <param name="sessionId">会话 ID</param>
    /// <returns>会话存在返回 true，否则返回 false</returns>
    public static bool IsAvailable(string sessionId)
    {
        return ClientSessions.Any(x => x.Id == sessionId);
    }
}
