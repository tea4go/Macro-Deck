using System.Net.WebSockets;

namespace SuchByte.MacroDeck.DataTypes;

/// <summary>
/// WebSocket 会话类，封装单个 WebSocket 连接的生命周期管理。
/// 负责接收和发送文本消息，并在连接断开或出错时触发相应事件。
/// 实现 IDisposable 接口以确保资源正确释放。
/// </summary>
public class WebSocketSession : IDisposable
{
    /// <summary>底层 WebSocket 连接实例</summary>
    private readonly WebSocket _webSocket;

    /// <summary>会话唯一标识（GUID）</summary>
    public string Id { get; }

    /// <summary>当收到文本消息时触发</summary>
    public event EventHandler<string>? TextMessageReceived;

    /// <summary>当连接断开时触发</summary>
    public event EventHandler? Disconnected;

    /// <summary>当发生错误时触发</summary>
    public event EventHandler<Exception>? Error;

    /// <summary>
    /// 构造函数，初始化 WebSocket 会话并生成唯一 ID
    /// </summary>
    /// <param name="webSocket">已建立的 WebSocket 连接</param>
    public WebSocketSession(WebSocket webSocket)
    {
        _webSocket = webSocket;
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 启动消息接收循环，在新线程中运行
    /// </summary>
    internal async Task Start()
    {
        await Task.Run(DoWork);
    }

    /// <summary>
    /// 消息接收主循环。持续监听 WebSocket 消息直到连接关闭或发生错误。
    /// 正常关闭或异常退出后都会触发 Disconnected 事件。
    /// </summary>
    private async Task DoWork()
    {
        try
        {
            while (_webSocket.State is WebSocketState.Open)
            {
                var message = await ReceiveStringAsync();
                if (message == null)
                {
                    return;
                }

                TextMessageReceived?.Invoke(this, message);
            }
        }
        catch (Exception ex)
        {
            Error?.Invoke(this, ex);
        }
        finally
        {
            await Close();
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 异步接收一条完整的文本消息。
    /// 使用 4KB 缓冲区分块接收，拼接直到收到完整消息。
    /// 仅接受文本和关闭类型的消息，其他类型抛出异常。
    /// </summary>
    /// <returns>接收到的文本消息，若连接正在关闭则返回 null</returns>
    private async Task<string?> ReceiveStringAsync()
    {
        var buffer = new byte[1024 * 4];
        var receivedMessage = new StringBuilder();

        WebSocketReceiveResult result;
        do
        {
            // 检测对端发起的关闭请求
            if (_webSocket.State == WebSocketState.CloseReceived)
            {
                return null;
            }

            var arraySegment = new ArraySegment<byte>(buffer);
            result = await _webSocket.ReceiveAsync(arraySegment, CancellationToken.None);

            // 仅允许文本和关闭消息类型
            if (result.MessageType is not WebSocketMessageType.Text and not WebSocketMessageType.Close)
            {
                throw new InvalidOperationException();
            }

            receivedMessage.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return receivedMessage.ToString();
    }

    /// <summary>
    /// 使用正常关闭原因关闭 WebSocket 连接
    /// </summary>
    public async Task Close()
    {
        await Close(new WebSocketNormalClose());
    }

    /// <summary>
    /// 使用指定的关闭原因关闭 WebSocket 连接
    /// </summary>
    /// <param name="reason">关闭原因，包含状态码和描述</param>
    public async Task Close(WebSocketCloseReason reason)
    {
        await Close(reason.Status, reason.Description);
    }

    /// <summary>
    /// 使用指定的状态码和描述关闭 WebSocket 连接。
    /// 仅在连接处于 Open 状态时执行关闭操作。
    /// </summary>
    private async Task Close(WebSocketCloseStatus webSocketCloseStatus, string statusDescription)
    {
        if (_webSocket.State is not WebSocketState.Open)
        {
            return;
        }

        await _webSocket.CloseAsync(webSocketCloseStatus,
            statusDescription,
            CancellationToken.None);
    }

    /// <summary>
    /// 发送文本消息（UTF-8 编码）
    /// </summary>
    /// <param name="message">要发送的文本消息</param>
    public async Task SendMessage(string message)
    {
        await SendMessage(Encoding.UTF8.GetBytes(message));
    }

    /// <summary>
    /// 发送二进制数据作为文本消息
    /// </summary>
    /// <param name="data">要发送的字节数据</param>
    private async Task SendMessage(byte[] data)
    {
        await _webSocket.SendAsync(new ArraySegment<byte>(data),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }

    /// <summary>
    /// 释放资源，关闭连接并释放 WebSocket
    /// </summary>
    public async void Dispose()
    {
        await Close();
        _webSocket.Dispose();
    }
}
