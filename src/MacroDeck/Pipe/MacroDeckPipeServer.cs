using System.IO;
using System.IO.Pipes;
using Serilog;

namespace SuchByte.MacroDeck.Pipe;

/// <summary>
/// 命名管道服务器，用于接收来自其他 Macro Deck 实例的管道消息。
/// 当用户尝试启动第二个实例时，通过管道通知已运行的实例显示主窗口。
/// 使用 "macrodeck" 作为管道名称，支持双向通信。
/// </summary>
public class MacroDeckPipeServer
{
    private static readonly ILogger Logger = Log.ForContext(typeof(MacroDeckPipeServer));

    /// <summary>收到管道消息时的回调事件</summary>
    public static event Action<string>? PipeMessage;

    /// <summary>
    /// 初始化管道服务器，在后台线程中持续监听管道连接。
    /// </summary>
    public static void Initialize()
    {
        Logger.Information("Initializing pipe server");
        Task.Run(async () => await HandleConnections().ConfigureAwait(false));
    }

    /// <summary>
    /// 持续处理管道连接。每次连接读取一行消息后断开，
    /// 然后等待下一个连接。循环无限执行。
    /// </summary>
    private static async Task HandleConnections()
    {
        do
        {
            await using var pipeServer = new NamedPipeServerStream("macrodeck", PipeDirection.InOut, 1);
            using var sr = new StreamReader(pipeServer);
            try
            {
                await pipeServer.WaitForConnectionAsync();
                pipeServer.WaitForPipeDrain();
                var result = await sr.ReadLineAsync();
                PipeMessage?.Invoke(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to handle pipe connection");
            }
            finally
            {
                if (pipeServer.IsConnected)
                {
                    pipeServer.Disconnect();
                }
            }
        } while (true);
    }
}
