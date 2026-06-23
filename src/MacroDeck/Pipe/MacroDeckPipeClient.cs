using System.IO.Pipes;

namespace SuchByte.MacroDeck.Pipe;

/// <summary>
/// 命名管道客户端，用于向已运行的 Macro Deck 实例发送消息。
/// 当检测到已有实例运行时，通过管道发送指令（如显示主窗口）。
/// </summary>
public static class MacroDeckPipeClient
{
    /// <summary>
    /// 向已运行的实例发送显示主窗口的消息
    /// </summary>
    /// <returns>发送成功返回 true，失败返回 false</returns>
    internal static Task<bool> SendShowMainWindowMessage()
    {
        return SendPipeMessage("show");
    }

    /// <summary>
    /// 通过命名管道发送消息到已运行的 Macro Deck 实例。
    /// 连接超时为 2 秒，超时后返回 false。
    /// </summary>
    /// <param name="message">要发送的消息字符串</param>
    /// <returns>发送成功返回 true，失败或超时返回 false</returns>
    private static async Task<bool> SendPipeMessage(string message)
    {
        try
        {
            await using var client = new NamedPipeClientStream("macrodeck");
            await client.ConnectAsync(2000);
            if (client.IsConnected)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                client.Write(bytes, 0, bytes.Length);
                return true;
            }
        }
        catch (TimeoutException)
        {
            // 超时忽略，说明没有已运行的实例
        }

        return false;
    }
}
