using System.Diagnostics;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// Macro Deck 日志管理器，提供统一的日志记录 API。
/// 支持按插件归属记录日志，核心事件使用 SuchByte.MacroDeck 作为 SourceContext，
/// 插件事件携带 Plugin 属性（被 Sentry 过滤排除）。
/// 日志级别可通过 LogLevel 属性运行时调整。
/// </summary>
public static class MacroDeckLogger
{
    private static readonly ILogger Logger = Log.ForContext(typeof(MacroDeckLogger));

    private static LogLevel _logLevel = Debugger.IsAttached ? LogLevel.Trace : LogLevel.Info;

    /// <summary>
    /// 运行时可调整的 Serilog 最低日志级别开关。
    /// 由日志配置（LoggingConfig）引用，通过 LogLevel 属性更新。
    /// </summary>
    public static readonly LoggingLevelSwitch LevelSwitch = new(ToLogEventLevel(_logLevel));

    /// <summary>
    /// 日志级别。设置时同步更新 Serilog 的最低级别开关。
    /// </summary>
    internal static LogLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            LevelSwitch.MinimumLevel = ToLogEventLevel(value);
            Logger.Information("Set log level to {LogLevel}", _logLevel);
        }
    }

    /// <summary>
    /// 将自定义 LogLevel 枚举转换为 Serilog 的 LogEventLevel。
    /// Nothing 级别使用 Fatal+1 来过滤所有日志。
    /// </summary>
    private static LogEventLevel ToLogEventLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Info => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            // 没有专门的"关闭"级别，使用高于 Fatal 的值过滤所有日志
            LogLevel.Nothing => LogEventLevel.Fatal + 1,
            _ => LogEventLevel.Information
        };
    }

    // -------------------------------------------------------------------------
    //  日志 API
    //
    //  使用这些方法记录所有日志。它们支持 Serilog 消息模板和结构化属性，
    //  可选异常和来源插件，例如：
    //
    //      MacroDeckLogger.Error(ex, "Error in {SomeParameter}", parameter);
    //      MacroDeckLogger.Information(plugin, "Connected to {Host}", host);
    //
    //  不指定插件时，事件归属于 Macro Deck 核心。
    // -------------------------------------------------------------------------

    /// <summary>
    /// 核心日志写入方法。根据是否指定插件设置不同的日志上下文：
    /// - 核心事件使用 SuchByte.MacroDeck 作为 SourceContext（通过 Sentry 白名单）
    /// - 插件事件携带 Plugin 属性（被 Sentry 排除）
    /// </summary>
    private static void Write(
        LogEventLevel level,
        MacroDeckPlugin? plugin,
        Exception? exception,
        string messageTemplate,
        object[] propertyValues)
    {
        var contextLogger = plugin is null
            ? Log.ForContext(Serilog.Core.Constants.SourceContextPropertyName, "SuchByte.MacroDeck")
            : Log.ForContext("Plugin", plugin.Name);
        contextLogger.Write(level, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录详细级别日志（核心）</summary>
    public static void Verbose(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Verbose, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录详细级别日志（核心，含异常）</summary>
    public static void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Verbose, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录详细级别日志（插件）</summary>
    public static void Verbose(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Verbose, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录详细级别日志（插件，含异常）</summary>
    public static void Verbose(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Verbose, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录调试级别日志（核心）</summary>
    public static void Debug(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Debug, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录调试级别日志（核心，含异常）</summary>
    public static void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Debug, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录调试级别日志（插件）</summary>
    public static void Debug(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Debug, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录调试级别日志（插件，含异常）</summary>
    public static void Debug(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Debug, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录信息级别日志（核心）</summary>
    public static void Information(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Information, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录信息级别日志（核心，含异常）</summary>
    public static void Information(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Information, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录信息级别日志（插件）</summary>
    public static void Information(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Information, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录信息级别日志（插件，含异常）</summary>
    public static void Information(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Information, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录警告级别日志（核心）</summary>
    public static void Warning(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Warning, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录警告级别日志（核心，含异常）</summary>
    public static void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Warning, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录警告级别日志（插件）</summary>
    public static void Warning(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Warning, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录警告级别日志（插件，含异常）</summary>
    public static void Warning(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Warning, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录错误级别日志（核心）</summary>
    public static void Error(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Error, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录错误级别日志（核心，含异常）</summary>
    public static void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Error, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录错误级别日志（插件）</summary>
    public static void Error(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Error, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录错误级别日志（插件，含异常）</summary>
    public static void Error(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Error, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录致命级别日志（核心）</summary>
    public static void Fatal(string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Fatal, null, null, messageTemplate, propertyValues);
    }

    /// <summary>记录致命级别日志（核心，含异常）</summary>
    public static void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Fatal, null, exception, messageTemplate, propertyValues);
    }

    /// <summary>记录致命级别日志（插件）</summary>
    public static void Fatal(MacroDeckPlugin plugin, string messageTemplate, params object[] propertyValues)
    {
        Write(LogEventLevel.Fatal, plugin, null, messageTemplate, propertyValues);
    }

    /// <summary>记录致命级别日志（插件，含异常）</summary>
    public static void Fatal(MacroDeckPlugin plugin,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        Write(LogEventLevel.Fatal, plugin, exception, messageTemplate, propertyValues);
    }

    /// <summary>
    /// 调试跟踪消息（已过时，请使用 MacroDeckLogger.Debug）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Debug(MacroDeckPlugin, string, params object[]) instead")]
    public static void Trace(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Debug("{LogMessage}", message);
    }

    /// <summary>
    /// 记录有用的信息（已过时，请使用 MacroDeckLogger.Information）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Information(MacroDeckPlugin, string, params object[]) instead")]
    public static void Info(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Information("{LogMessage}", message);
    }

    /// <summary>
    /// 记录警告（已过时，请使用 MacroDeckLogger.Warning）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Warning(MacroDeckPlugin, string, params object[]) instead")]
    public static void Warning(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Warning("{LogMessage}", message);
    }

    /// <summary>
    /// 记录错误（已过时，请使用 MacroDeckLogger.Error）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Error(MacroDeckPlugin, string, params object[]) instead")]
    public static void Error(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Error("{LogMessage}", message);
    }

    /// <summary>
    /// 调试跟踪消息（已过时，请使用 MacroDeckLogger.Debug）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Debug(MacroDeckPlugin, string, params object[]) instead")]
    public static void Trace(MacroDeckPlugin macroDeckPlugin, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Debug("{LogMessage}", message);
    }

    /// <summary>
    /// 记录有用的信息（已过时，请使用 MacroDeckLogger.Information）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Information(MacroDeckPlugin, string, params object[]) instead")]
    public static void Info(MacroDeckPlugin macroDeckPlugin, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Information("{LogMessage}", message);
    }

    /// <summary>
    /// 记录警告（已过时，请使用 MacroDeckLogger.Warning）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Warning(MacroDeckPlugin, string, params object[]) instead")]
    public static void Warning(MacroDeckPlugin macroDeckPlugin, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Warning("{LogMessage}", message);
    }

    /// <summary>
    /// 记录错误（已过时，请使用 MacroDeckLogger.Error）
    /// </summary>
    [Obsolete("Use MacroDeckLogger.Error(MacroDeckPlugin, string, params object[]) instead")]
    public static void Error(MacroDeckPlugin macroDeckPlugin, string message)
    {
        Log.ForContext("Plugin", macroDeckPlugin.Name)
            .Error("{LogMessage}", message);
    }

    /// <summary>
    /// 清理日志目录中超过 30 天的日志文件
    /// </summary>
    internal static void CleanUpLogsDir()
    {
        foreach (var file in new DirectoryInfo(ApplicationPaths.LogsDirectoryPath).GetFiles()
            .Where(p => p.CreationTime < DateTime.Now.AddDays(-30)).ToArray())
        {
            try
            {
                File.Delete(file.FullName);
            }
            catch
            {
            }
        }
    }
}

/// <summary>
/// 日志级别枚举，定义不同的日志详细程度
/// </summary>
public enum LogLevel
{
    /// <summary>记录跟踪、信息、警告和错误</summary>
    Trace = 1,

    /// <summary>记录信息、警告和错误</summary>
    Info = 2,

    /// <summary>记录警告和错误</summary>
    Warning = 3,

    /// <summary>仅记录错误</summary>
    Error = 4,

    /// <summary>不记录任何日志</summary>
    Nothing = 100
}
