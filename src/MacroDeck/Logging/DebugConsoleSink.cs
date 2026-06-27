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

        var colors = ColorForLevel(logEvent.Level);

        var console = DebugConsole.Current;
        if (console is null)
        {
            lock (BufferLock)
            {
                Buffer.Enqueue(new BufferedEntry(text, source, colors));
                while (Buffer.Count > BufferCapacity)
                {
                    Buffer.Dequeue();
                }
            }
            return;
        }

        console.AppendText(text, source, colors);
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
            console.AppendText(entry.Text, entry.Source, entry.Colors);
        }
    }

    /// <summary>
    /// 存储日志级别对应的背景色与前景（文字）色对。
    /// </summary>
    public readonly struct LevelColors
    {
        public Color Background { get; }
        public Color Foreground { get; }

        public LevelColors(Color background, Color foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        /// <summary>是否有有效的背景色（非 Empty 且不透明）</summary>
        public bool HasBackground => Background != Color.Empty && Background.A > 0;
    }

    /// <summary>
    /// 根据 Serilog 日志级别返回对应的背景色与前景色组合。
    /// 背景色用于在调试控制台中标记整行条目的严重程度；前景色控制文字颜色以确保可读性。
    /// </summary>
    private static LevelColors ColorForLevel(LogEventLevel level)
    {
        return level switch
        {
            // 致命错误：深红背景 + 亮红文字，最高视觉优先级
            LogEventLevel.Fatal => new LevelColors(
                Color.FromArgb(120, 15, 15),
                Color.FromArgb(255, 85, 85)),
            // 一般错误：暗红背景 + 柔和红色文字，与 Fatal 区分
            LogEventLevel.Error => new LevelColors(
                Color.FromArgb(90, 20, 20),
                Color.FromArgb(255, 105, 105)),
            // 警告：暗金背景 + 金黄色文字，醒目且柔和
            LogEventLevel.Warning => new LevelColors(
                Color.FromArgb(90, 65, 10),
                Color.FromArgb(255, 195, 60)),
            // 信息：暗绿背景 + 白色文字，常规信息清晰可读
            LogEventLevel.Information => new LevelColors(
                Color.FromArgb(25, 55, 25),
                Color.White),
            // 调试：暗灰背景 + 浅灰色文字，与 Info 区分
            LogEventLevel.Debug => new LevelColors(
                Color.FromArgb(45, 45, 45),
                Color.FromArgb(170, 170, 170)),
            // 详细：无背景 + 暗灰色文字，最低优先级
            LogEventLevel.Verbose => new LevelColors(
                Color.Empty,
                Color.FromArgb(100, 100, 100)),
            _ => new LevelColors(Color.Empty, Color.White)
        };
    }

    private readonly struct BufferedEntry
    {
        public BufferedEntry(string text, string source, LevelColors colors)
        {
            Text = text;
            Source = source;
            Colors = colors;
        }

        public string Text { get; }
        public string Source { get; }
        public LevelColors Colors { get; }
    }
}
