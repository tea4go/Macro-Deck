namespace SuchByte.MacroDeck.Events;

/// <summary>
/// Macro Deck 事件参数类，携带事件触发时的上下文信息
/// </summary>
public class MacroDeckEventArgs : EventArgs
{
    /// <summary>触发事件的动作按钮</summary>
    public ActionButton.ActionButton ActionButton { get; set; }

    /// <summary>事件参数</summary>
    public object Parameter { get; set; }
}

/// <summary>
/// Macro Deck 事件接口，所有系统事件必须实现此接口。
/// 插件通过实现此接口来注册自定义事件。
/// </summary>
public interface IMacroDeckEvent
{
    /// <summary>事件名称</summary>
    string Name { get; }

    /// <summary>事件触发时的处理委托</summary>
    EventHandler<MacroDeckEventArgs> OnEvent { get; set; }

    /// <summary>事件参数建议值列表，用于 UI 中显示可选参数</summary>
    List<string> ParameterSuggestions { get; set; }
}
