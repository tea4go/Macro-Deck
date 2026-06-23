using System.Diagnostics;
using Serilog;
using SuchByte.MacroDeck.Pipe;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck;

/// <summary>
/// 应用程序入口类，负责初始化 Windows Forms 应用程序、
/// 注册异常处理、检查单实例运行、初始化日志和启动 Macro Deck。
/// </summary>
internal class Program
{
    private static readonly ILogger Logger = Log.ForContext(typeof(Program));

    /// <summary>
    /// 应用程序主入口点。
    /// </summary>
    /// <param name="args">命令行参数</param>
    [STAThread]
    private static void Main(string[] args)
    {
        // 初始化 Windows Forms 应用程序视觉样式
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 注册异常事件处理器，确保所有未处理异常都被记录
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += ApplicationThreadException;

        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        // 解析命令行启动参数
        var startParameters = StartParameters.ParseParameters(args);

        // 检查是否已有实例运行（单实例模式）
        CheckRunningInstance(startParameters.IgnorePidCheck).Wait();

        // 初始化应用程序路径（根据是否为便携模式）
        ApplicationPaths.Initialize(startParameters.PortableMode);

        // 提前构建 Serilog 日志记录器，确保从应用启动开始就有日志输出
        // ASP.NET 主机后续会复用这个静态日志记录器
        Log.Logger = LoggingConfig.CreateLogger();

        // 启动 Macro Deck 主程序
        MacroDeck.Start(startParameters);
    }

    /// <summary>
    /// 检查是否已有 Macro Deck 实例正在运行。
    /// 如果已有实例运行，尝试通过命名管道发送显示主窗口消息；
    /// 如果管道通信失败，则强制终止其他实例。
    /// </summary>
    /// <param name="ignoredPid">要忽略的进程 ID（重启时使用）</param>
    private static async Task CheckRunningInstance(int ignoredPid)
    {
        var proc = Process.GetCurrentProcess();
        var processes = Process.GetProcessesByName(proc.ProcessName)
            .Where(x => ignoredPid == 0 || x.Id != ignoredPid)
            .ToArray();

        // 没有其他实例运行，正常启动
        if (processes.Length <= 1)
        {
            return;
        }

        // 尝试通过管道通知已有实例显示主窗口
        if (await MacroDeckPipeClient.SendShowMainWindowMessage())
        {
            Environment.Exit(0);
            return;
        }

        // 管道通信失败，强制终止其他实例
        foreach (var p in processes.Where(x => x.Id != proc.Id))
        {
            try
            {
                p.Kill();
            }
            catch
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// AppDomain 未处理异常事件处理，记录错误日志
    /// </summary>
    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Error(e.ExceptionObject as Exception,
            "Unhandled domain exception: {ExceptionObject}",
            e.ExceptionObject);
    }

    /// <summary>
    /// Windows Forms 线程未处理异常事件处理，记录错误日志
    /// </summary>
    private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
    {
        Logger.Error(e.Exception, "Unhandled thread exception");
    }
}
