using System.IO;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Themes;
using SuchByte.MacroDeck.Logging;

namespace SuchByte.MacroDeck.StartupConfig;

/// <summary>
/// 日志配置类。负责构建应用程序级别的 Serilog 日志记录器，
/// 包括控制台输出、文件输出、调试控制台输出以及可选的 Sentry 错误上报。
/// 日志记录器在启动早期创建并赋值给 Log.Logger，ASP.NET 主机通过 ConfigureSerilog 重用同一静态记录器。
/// </summary>
public static class LoggingConfig
{
    private const string OutputTemplate =
        "{Timestamp:HH:mm:ss} ({SourceFile}:{SourceLine}) [{Level:u1}]> {Source} - {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// 构建应用程序级别的 Serilog 日志记录器。
    /// 在启动早期调用，确保日志记录从一开始就可用；ASP.NET 主机通过 ConfigureSerilog 重用该静态记录器。
    /// </summary>
    /// <returns>配置好的 Serilog ILogger 实例。</returns>
    public static ILogger CreateLogger()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.With<PluginSourceEnricher>()
            .Enrich.With<CallerInfoEnricher>()
            .MinimumLevel.ControlledBy(MacroDeckLogger.LevelSwitch)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                outputTemplate: OutputTemplate)
            .WriteTo.File(Path.Combine(ApplicationPaths.LogsDirectoryPath, "log.txt"),
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 53477376, // 50 MB
                outputTemplate: OutputTemplate)
            .WriteTo.Sink(new DebugConsoleSink(new MessageTemplateTextFormatter(OutputTemplate, null)));

        // 匿名错误上报。仅当 CI/CD 注入了有效 DSN 时才注册；条件路由只上报 Macro Deck 自身事件（不包含插件），且遵循用户的选择退出设置。
        if (SentryConfiguration.IsDsnConfigured)
        {
            loggerConfiguration.WriteTo.Conditional(SentryConfiguration.ShouldSend,
                wt => wt.Sentry(SentryConfiguration.Configure));
        }

        return loggerConfiguration.CreateLogger();
    }

    /// <summary>
    /// 配置 ASP.NET 主机使用 Serilog 作为日志提供程序。
    /// 将静态 Log.Logger 集成到 ASP.NET 的依赖注入和日志管道中。
    /// </summary>
    /// <param name="hostBuilder">ASP.NET Core 主机构建器。</param>
    /// <returns>配置后的 IHostBuilder 实例。</returns>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog();
    }
}
