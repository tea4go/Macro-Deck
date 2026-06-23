using SuchByte.MacroDeck.Plugins;

namespace SuchByte.MacroDeck.Events;

/// <summary>
/// 事件监听器类，将系统事件与动作列表关联。
/// 当指定事件触发且参数匹配时，执行关联的动作列表。
/// </summary>
public class EventListener
{
    /// <summary>要监听的事件名称</summary>
    public string EventToListen { get; set; }

    /// <summary>事件参数匹配值</summary>
    public string Parameter { get; set; } = "";

    /// <summary>事件触发时执行的动作列表</summary>
    public List<PluginAction?> Actions = new();
}
