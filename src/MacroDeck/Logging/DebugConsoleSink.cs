using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SuchByte.MacroDeck.GUI.Dialogs;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// Serilog 日志接收器，将日志事件转发到调试控制台窗口。
/// 当没有调试控制台打开时，日志被写入环形缓冲区；窗口打开后通过 <see cref="Replay"/> 回放。
/// </summary>
public class DebugConsoleSink : ILogEventSink
{
    /// <summary>环形缓冲最大条目数，超出后丢弃最早条目</summary>
    private const int BufferCapacity = 1000;

    private static readonly object BufferLock = new();
    private static readonly Queue<BufferedEntry> Buffer = new();

    private readonly ITextFormatter _formatter;

    public DebugConsoleSink(ITextFormatter formatter)
    {
        _formatter = formatter;
    }

    /// <summary>
    /// 发送日志事件到调试控制台。
    /// 若当前没有调试控制台（窗口未开），写入静态环形缓冲区；否则直接转发。
    /// </summary>
    public void Emit(LogEvent logEvent)
    {
        using var writer = new StringWriter();
        _formatter.Format(logEvent, writer);
        var text = writer.ToString();

        var source = logEvent.Properties.TryGetValue("Source", out var value) &&
            value is ScalarValue { Value: string name }
                ? name
                : "MacroDeck";

        var color = ColorForLevel(logEvent.Level);

        var console = DebugConsole.Current;
        if (console is null)
        {
            lock (BufferLock)
            {
                Buffer.Enqueue(new BufferedEntry(text, source, color));
                while (Buffer.Count > BufferCapacity)
                {
                    Buffer.Dequeue();
                }
            }
            return;
        }

        console.AppendText(text, source, color);
    }

    /// <summary>
    /// 将缓冲区中的所有日志回放到当前调试控制台，并清空缓冲区。
    /// 应在窗口句柄已创建后调用（如 <see cref="DebugConsole.OnLoad"/>）。
    /// 若当前没有调试控制台，则不做任何操作。
    /// </summary>
    public static void Replay()
    {
        var console = DebugConsole.Current;
        if (console is null)
        {
            return;
        }

        BufferedEntry[] snapshot;
        lock (BufferLock)
        {
            snapshot = Buffer.ToArray();
            Buffer.Clear();
        }

        foreach (var entry in snapshot)
        {
            console.AppendText(entry.Text, entry.Source, entry.Color);
        }
    }

    /// <summary>
    /// 根据 Serilog 日志级别返回对应的文字颜色。
    /// 配色针对深色背景（~#1E1E1E）优化，以柔和暖色调为主，减少视觉疲劳。
    /// 严格按 Fatal > Error > Warning > Info > Debug > Verbose 递减视觉权重。
    /// </summary>
    private static Color ColorForLevel(LogEventLevel level)
    {
        return level switch
        {
            // 致命错误：暖珊瑚红，最高视觉优先级，与 Error 明显区分
            LogEventLevel.Fatal => Color.FromArgb(255, 107, 107),
            // 一般错误：柔和鲑鱼红，比 Fatal 亮度更高、饱和度更低
            LogEventLevel.Error => Color.FromArgb(255, 138, 128),
            // 警告：温暖琥珀色，醒目但不刺痛
            LogEventLevel.Warning => Color.FromArgb(255, 213, 79),
            // 信息：柔和米白，长时间阅读不刺眼
            LogEventLevel.Information => Color.FromArgb(235, 235, 235),
            // 调试：中等灰色，低调可读
            LogEventLevel.Debug => Color.FromArgb(158, 158, 158),
            // 详细：浅暗灰，最低优先级但确保深色背景下可见
            LogEventLevel.Verbose => Color.FromArgb(135, 135, 135),
            _ => Color.FromArgb(235, 235, 235)
        };
    }

    private readonly struct BufferedEntry
    {
        public BufferedEntry(string text, string source, Color color)
        {
            Text = text;
            Source = source;
            Color = color;
        }

        public string Text { get; }
        public string Source { get; }
        public Color Color { get; }
    }
}
