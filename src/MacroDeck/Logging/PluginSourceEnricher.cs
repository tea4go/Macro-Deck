using Serilog.Core;
using Serilog.Events;

namespace SuchByte.MacroDeck.Logging;

/// <summary>
/// 日志事件丰富器，为每条日志事件添加 Source 属性。
/// 如果日志来自插件，Source 为插件名称；否则为 "MacroDeck"。
/// 插件日志事件通过 MacroDeckLogger 设置的 Plugin 属性识别。
/// </summary>
public class PluginSourceEnricher : ILogEventEnricher
{
    /// <summary>Source 属性名称</summary>
    public const string SourcePropertyName = "Source";

    private const string MacroDeckSource = "MacroDeck";
    private const string PluginPropertyName = "Plugin";

    /// <summary>
    /// 丰富日志事件，添加 Source 属性。
    /// 检查日志事件是否包含 Plugin 属性，有则使用插件名作为来源。
    /// </summary>
    /// <param name="logEvent">日志事件</param>
    /// <param name="propertyFactory">属性工厂</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var source = MacroDeckSource;
        if (logEvent.Properties.TryGetValue(PluginPropertyName, out var pluginValue) &&
            pluginValue is ScalarValue { Value: string pluginName } &&
            !string.IsNullOrWhiteSpace(pluginName))
        {
            source = pluginName;
        }

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(SourcePropertyName, source));
    }
}
