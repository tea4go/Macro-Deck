using Newtonsoft.Json;
using SuchByte.MacroDeck.Events;
using SuchByte.MacroDeck.Hotkeys;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.ActionButton;

/// <summary>
/// 动作按钮类，表示 Macro Deck 上的一个可交互按钮。
/// 每个按钮拥有开/关两种状态的图标、颜色、标签，以及多种触发类型的动作列表。
/// 支持热键绑定和变量状态绑定。
/// </summary>
public class ActionButton : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// 构造函数，注册变量变化事件监听器
    /// </summary>
    public ActionButton()
    {
        VariableManager.OnVariableChanged += VariableChanged;
    }

    /// <summary>
    /// 更新热键注册，如果按钮配置了热键则注册到热键管理器
    /// </summary>
    public void UpdateHotkey()
    {
        if (KeyCode != Keys.None)
        {
            HotkeyManager.AddHotkey(this, ModifierKeyCodes, KeyCode);
        }
    }

    /// <summary>
    /// 更新状态绑定，根据绑定的变量值更新按钮的开/关状态
    /// </summary>
    public void UpdateBindingState()
    {
        if (!string.IsNullOrWhiteSpace(StateBindingVariable))
        {
            var variable = VariableManager.ListVariables.FirstOrDefault(v => v.Name == StateBindingVariable);
            if (variable != null)
            {
                UpdateBindingState(variable);
            }
        }
    }

    /// <summary>
    /// 指示当前实例是否已被释放
    /// </summary>
    [JsonIgnore] public bool IsDisposed => _disposed;

    /// <summary>
    /// 释放资源的核心方法，移除热键绑定、取消变量监听、通知所有动作按钮被删除
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
        }

        // 移除热键注册
        HotkeyManager.RemoveHotkey(this);

        // 取消变量变化事件订阅
        VariableManager.OnVariableChanged -= VariableChanged;

        // 通知所有动作按钮已被删除，以便插件清理资源
        foreach (var pluginAction in Actions)
        {
            pluginAction?.OnActionButtonDelete();
        }
        
        foreach (var pluginAction in EventListeners.SelectMany(eventListeners => eventListeners.Actions))
        {
            pluginAction.OnActionButtonDelete();
        }

        _disposed = true;
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 析构函数，确保非托管资源被释放
    /// </summary>
    ~ActionButton()
    {
        Dispose(false);
    }

    /// <summary>
    /// 变量变化事件处理，当绑定的变量值发生变化时更新按钮状态
    /// </summary>
    private void VariableChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(StateBindingVariable))
        {
            return;
        }

        if (sender is Variable variable)
        {
            UpdateBindingState(variable);
        }
    }

    /// <summary>
    /// 根据变量值更新按钮的绑定状态。
    /// 支持 "true"/"false" 和 "on"/"off" 两种布尔值表示方式。
    /// </summary>
    /// <param name="variable">发生变化的变量</param>
    private void UpdateBindingState(Variable variable)
    {
        if (!variable.Name.Equals(StateBindingVariable))
        {
            return;
        }

        _ = bool.TryParse(variable.Value, out var newState);

        // 支持 "on" 作为 true 的别名
        if (variable.Value.ToLower().Equals("on"))
        {
            newState = true;
        }

        State = newState;
    }

    /// <summary>
    /// 按钮的唯一标识符
    /// </summary>
    public string Guid { get; set; } = System.Guid.CreateVersion7().ToString();

    /// <summary>
    /// 按钮状态变化事件
    /// </summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// 按钮图标变化事件
    /// </summary>
    public event EventHandler? IconChanged;

    /// <summary>
    /// 按钮的开关状态。设置时如果状态发生变化，
    /// 会通知服务器更新并触发 StateChanged 事件。
    /// </summary>
    public bool State
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            MacroDeckServer.UpdateState(this);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 按钮关闭状态的图标 Base64 字符串
    /// </summary>
    public string IconOff
    {
        get;

        set
        {
            field = value;
            IconChanged?.Invoke(this, EventArgs.Empty);
        }
    } = string.Empty;

    /// <summary>
    /// 按钮开启状态的图标 Base64 字符串
    /// </summary>
    public string IconOn
    {
        get;

        set
        {
            field = value;
            IconChanged?.Invoke(this, EventArgs.Empty);
        }
    } = string.Empty;

    /// <summary>
    /// 按钮关闭状态的背景色
    /// </summary>
    public Color BackColorOff
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            MacroDeckServer.UpdateState(this);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    } = Color.FromArgb(65, 65, 65);

    /// <summary>
    /// 按钮开启状态的背景色
    /// </summary>
    public Color BackColorOn
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            MacroDeckServer.UpdateState(this);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    } = Color.FromArgb(65, 65, 65);

    /// <summary>
    /// 按钮关闭状态的标签
    /// </summary>
    public ButtonLabel? LabelOff { get; set; } = new();

    /// <summary>
    /// 按钮开启状态的标签
    /// </summary>
    public ButtonLabel? LabelOn { get; set; } = new();

    /// <summary>
    /// 按钮在网格中的 X 坐标位置，-1 表示未定位
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int Position_X { get; set; } = -1;

    /// <summary>
    /// 按钮在网格中的 Y 坐标位置，-1 表示未定位
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int Position_Y { get; set; } = -1;

    /// <summary>
    /// 用于绑定按钮状态的变量名称，当变量值变化时自动更新按钮开关状态
    /// </summary>
    public string StateBindingVariable { get; set; } = string.Empty;

    /// <summary>
    /// 短按时执行的动作列表
    /// </summary>
    public List<PluginAction?> Actions { get; set; } = new();

    /// <summary>
    /// 短按释放时执行的动作列表
    /// </summary>
    public List<PluginAction?> ActionsRelease { get; set; } = new();

    /// <summary>
    /// 长按时执行的动作列表
    /// </summary>
    public List<PluginAction?> ActionsLongPress { get; set; } = new();

    /// <summary>
    /// 长按释放时执行的动作列表
    /// </summary>
    public List<PluginAction?> ActionsLongPressRelease { get; set; } = new();

    /// <summary>
    /// 事件监听器列表，用于响应系统事件并触发对应动作
    /// </summary>
    public List<EventListener> EventListeners { get; set; } = new();

    /// <summary>
    /// 热键修饰键（Ctrl、Shift、Alt 等）
    /// </summary>
    public Keys ModifierKeyCodes { get; set; } = Keys.None;

    /// <summary>
    /// 热键键码
    /// </summary>
    public Keys KeyCode { get; set; } = Keys.None;
}
