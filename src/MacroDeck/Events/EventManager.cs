namespace SuchByte.MacroDeck.Events;

/// <summary>
/// 事件管理器，负责注册和管理所有系统事件，并在事件触发时执行对应的动作。
/// 事件由插件注册，当事件触发时，管理器会查找所有匹配的事件监听器并执行其动作。
/// </summary>
public static class EventManager
{
    /// <summary>
    /// 已注册的事件列表
    /// </summary>
    private static List<IMacroDeckEvent> _registeredEvents = new();

    /// <summary>
    /// 获取所有已注册的事件列表
    /// </summary>
    public static List<IMacroDeckEvent> RegisteredEvents => _registeredEvents;

    /// <summary>
    /// 注册一个系统事件。如果事件尚未注册，则添加到列表并订阅其触发事件。
    /// </summary>
    /// <param name="macroDeckEvent">要注册的事件实例</param>
    public static void RegisterEvent(IMacroDeckEvent macroDeckEvent)
    {
        if (!_registeredEvents.Contains(macroDeckEvent))
        {
            _registeredEvents.Add(macroDeckEvent);
            macroDeckEvent.OnEvent += OnActionButtonEventTrigger;
        }
    }

    /// <summary>
    /// 根据事件名称获取已注册的事件实例
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <returns>事件实例，未找到则返回 null</returns>
    public static IMacroDeckEvent GetEventByName(string name)
    {
        var macroDeckEvent = _registeredEvents.Find(macroDeckEvent => macroDeckEvent.Name.Equals(name));
        return macroDeckEvent;
    }

    /// <summary>
    /// 事件触发时的处理方法。
    /// 查找所有匹配事件名称和参数的监听器，并异步执行其关联的动作。
    /// </summary>
    /// <param name="sender">触发事件的 IMacroDeckEvent 实例</param>
    /// <param name="e">事件参数，包含触发源动作按钮和参数</param>
    private static void OnActionButtonEventTrigger(object sender, MacroDeckEventArgs e)
    {
        Task.Run(() =>
        {
            var macroDeckEvent = (IMacroDeckEvent)sender;
            var actionButton = e.ActionButton;

            // 查找所有事件名称和参数匹配的监听器，并执行其动作
            foreach (var action in actionButton.EventListeners.FindAll(x =>
                    x.EventToListen.Equals(macroDeckEvent.Name) &&
                    x.Parameter.ToLower()
                        .Equals(e.Parameter.ToString()
                            .ToLower()))
                .SelectMany(eventListener => eventListener.Actions))
            {
                action.Trigger("-1", e.ActionButton);
            }
        });
    }
}
