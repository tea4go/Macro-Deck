using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SuchByte.MacroDeck.GUI.Dialogs;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// Serilog 日志接收器，将日志事件转发到调试控制台窗口。
/// 当没有调试控制台打开时为空操作，因此可以永久保留在日志管道中。
/// </summary>
public class DebugConsoleSink : ILogEventSink
{
    private readonly ITextFormatter _formatter;

    /// <summary>
    /// 构造函数，指定日志格式化器
    /// </summary>
    /// <param name="formatter">日志文本格式化器</param>
    public DebugConsoleSink(ITextFormatter formatter)
    {
        _formatter = formatter;
    }

    /// <summary>
    /// 发送日志事件到调试控制台。
    /// 如果当前没有打开的调试控制台，则直接返回。
    /// 提取日志来源（Source 属性）并根据日志级别设置显示颜色。
    /// </summary>
    /// <param name="logEvent">日志事件</param>
    public void Emit(LogEvent logEvent)
    {
        var console = DebugConsole.Current;
        if (console is null)
        {
            return;
        }

        using var writer = new StringWriter();
        _formatter.Format(logEvent, writer);

        // 从日志属性中提取来源，默认为 "MacroDeck"
        var source = logEvent.Properties.TryGetValue("Source", out var value) &&
            value is ScalarValue { Value: string name }
                ? name
                : "MacroDeck";

        console.AppendText(writer.ToString(), source, ColorForLevel(logEvent.Level));
    }

    /// <summary>
    /// 根据日志级别返回对应的显示颜色
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns>显示颜色</returns>
    private static Color ColorForLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Fatal => Color.FromArgb(255, 99, 99),
            LogEventLevel.Error => Color.FromArgb(255, 99, 99),
            LogEventLevel.Warning => Color.Orange,
            LogEventLevel.Information => Color.White,
            LogEventLevel.Debug => Color.Gray,
            LogEventLevel.Verbose => Color.DarkGray,
            _ => Color.White
        };
    }
}
