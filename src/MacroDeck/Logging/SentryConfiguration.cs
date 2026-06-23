using System.Diagnostics;
using Sentry.Serilog;
using Serilog.Events;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// Sentry 错误报告配置类，负责配置 Sentry SDK 的初始化选项、
/// 事件过滤、隐私数据清理和面包屑处理。
/// 仅上报来自 Macro Deck 核心的错误事件，排除插件产生的日志。
/// </summary>
public static class SentryConfiguration
{
    /// <summary>DSN 占位符，在 CI/CD 中替换为真实 DSN。不要提交真实的 DSN。</summary>
    private const string Dsn = "__SENTRY_DSN__";

    /// <summary>Macro Deck 核心命名空间前缀，用于过滤日志来源</summary>
    private const string SourceNamespace = "SuchByte.MacroDeck";

    /// <summary>Windows 用户配置文件路径，用于隐私数据清理</summary>
    private static readonly string UserProfilePath =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <summary>Windows 用户名，用于隐私数据清理</summary>
    private static readonly string UserName = Environment.UserName;

    /// <summary>是否启用 Sentry 错误报告</summary>
    public static bool Enabled = true;

    /// <summary>DSN 是否已配置（以 http 开头表示已配置）</summary>
    public static bool IsDsnConfigured => Dsn.StartsWith("http", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 配置 Sentry Serilog 选项。
    /// 设置最低事件级别为 Error，面包屑级别为 Warning，
    /// 禁止发送个人身份信息，附加堆栈跟踪，并设置事件发送前和面包屑的前置处理器。
    /// </summary>
    /// <param name="options">Sentry Serilog 配置选项</param>
    public static void Configure(SentrySerilogOptions options)
    {
        options.Dsn = Dsn;
        options.MinimumEventLevel = LogEventLevel.Error;
        options.MinimumBreadcrumbLevel = LogEventLevel.Warning;
        options.SendDefaultPii = false;
        options.ServerName = null;
        options.AttachStacktrace = true;
        options.Release
            = $"macro-deck@{typeof(SentryConfiguration).Assembly.GetName().Version?.ToString() ?? "unknown"}";
        options.Environment = Debugger.IsAttached ? "debug" : "release";
        options.SetBeforeSend(BeforeSend);
        options.SetBeforeBreadcrumb(BeforeBreadcrumb);
    }

    /// <summary>
    /// 判断是否应该发送日志事件到 Sentry。
    /// 仅发送已启用且来自 Macro Deck 核心的事件。
    /// </summary>
    /// <param name="logEvent">日志事件</param>
    /// <returns>是否发送</returns>
    public static bool ShouldSend(LogEvent logEvent)
    {
        return Enabled && IsMacroDeckEvent(logEvent);
    }

    /// <summary>
    /// 判断日志事件是否来自 Macro Deck 核心。
    /// 包含 Plugin 属性的事件（来自插件）被排除；
    /// 仅允许 SourceContext 以 "SuchByte.MacroDeck" 开头的事件通过。
    /// </summary>
    private static bool IsMacroDeckEvent(LogEvent logEvent)
    {
        // 显式标记为插件的事件被排除
        if (logEvent.Properties.ContainsKey("Plugin"))
        {
            return false;
        }

        // 严格白名单：仅允许 Macro Deck 核心命名空间的事件
        return logEvent.Properties.TryGetValue(Serilog.Core.Constants.SourceContextPropertyName, out var value) &&
            value is ScalarValue { Value: string sourceContext } &&
            sourceContext.StartsWith(SourceNamespace, StringComparison.Ordinal);
    }

    /// <summary>
    /// Sentry 事件发送前的处理器。
    /// 清除服务器名称，清理消息和异常中的隐私数据（用户路径和用户名）。
    /// </summary>
    /// <param name="sentryEvent">Sentry 事件</param>
    /// <param name="hint">Sentry 提示</param>
    /// <returns>处理后的事件，如果禁用则返回 null</returns>
    private static SentryEvent? BeforeSend(SentryEvent sentryEvent, SentryHint hint)
    {
        if (!Enabled)
        {
            return null;
        }

        // 清除服务器名称以保护隐私
        sentryEvent.ServerName = null;

        // 清理消息中的隐私数据
        if (sentryEvent.Message is { } message)
        {
            sentryEvent.Message = new SentryMessage
            {
                Message = Scrub(message.Message),
                Formatted = Scrub(message.Formatted),
                Params = message.Params
            };
        }

        // 清理异常信息和堆栈跟踪中的隐私数据
        if (sentryEvent.SentryExceptions is { } exceptions)
        {
            foreach (var exception in exceptions)
            {
                exception.Value = Scrub(exception.Value);
                if (exception.Stacktrace?.Frames is { } frames)
                {
                    foreach (var frame in frames)
                    {
                        frame.FileName = Scrub(frame.FileName);
                        frame.AbsolutePath = Scrub(frame.AbsolutePath);
                    }
                }
            }
        }

        return sentryEvent;
    }

    /// <summary>
    /// 面包屑发送前的处理器。
    /// 清理面包屑消息和数据中的隐私数据。
    /// </summary>
    /// <param name="breadcrumb">面包屑</param>
    /// <param name="hint">Sentry 提示</param>
    /// <returns>处理后的面包屑，如果禁用则返回 null</returns>
    private static Breadcrumb? BeforeBreadcrumb(Breadcrumb breadcrumb, SentryHint hint)
    {
        if (!Enabled)
        {
            return null;
        }

        IReadOnlyDictionary<string, string>? data = null;
        if (breadcrumb.Data is { } original)
        {
            data = original.ToDictionary(pair => pair.Key, pair => Scrub(pair.Value) ?? string.Empty);
        }

        // 面包屑是不可变的，需要用清理后的值重新构建
        return new Breadcrumb(Scrub(breadcrumb.Message),
            breadcrumb.Type,
            data,
            breadcrumb.Category,
            breadcrumb.Level);
    }

    /// <summary>
    /// 从字符串中移除 Windows 用户配置文件路径和用户名，保护隐私。
    /// 将用户路径替换为 %USERPROFILE%，用户名替换为 [user]。
    /// </summary>
    /// <param name="value">要清理的字符串</param>
    /// <returns>清理后的字符串</returns>
    private static string? Scrub(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (!string.IsNullOrEmpty(UserProfilePath))
        {
            value = value.Replace(UserProfilePath, "%USERPROFILE%", StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrEmpty(UserName))
        {
            value = value.Replace(UserName, "[user]", StringComparison.OrdinalIgnoreCase);
        }

        return value;
    }
}
