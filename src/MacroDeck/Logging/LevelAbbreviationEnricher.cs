using Serilog.Core;
using Serilog.Events;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// 日志事件丰富器，为每条日志添加自定义的 LevelAbbr 属性。
/// 使用与标准 Serilog 首字母不同的自定义缩写映射，
/// 使控制台输出中的级别标识更直观、紧凑。
/// </summary>
public class LevelAbbreviationEnricher : ILogEventEnricher
{
    /// <summary>自定义级别缩写属性键</summary>
    public const string PropertyName = "LevelAbbr";

    /// <summary>
    /// 丰富日志事件，根据日志级别添加 LevelAbbr 属性。
    /// </summary>
    /// <param name="logEvent">日志事件</param>
    /// <param name="propertyFactory">属性工厂</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var abbr = logEvent.Level switch
        {
            LogEventLevel.Fatal       => "M",  // Emergency
            LogEventLevel.Error       => "E",  // Error
            LogEventLevel.Warning     => "W",  // Warning
            LogEventLevel.Information => "N",  // Notice
            LogEventLevel.Debug       => "D",  // Debug
            LogEventLevel.Verbose     => "D",  // Debug (verbose 并入 debug)
            _                         => "?"
        };

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(PropertyName, abbr));
    }
}
