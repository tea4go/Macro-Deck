using System.Reflection;
using Newtonsoft.Json;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// Macro Deck 插件抽象基类，所有插件必须继承此类。
/// 提供插件的名称、版本、作者、动作列表、图标和配置等基础功能。
/// </summary>
public abstract class MacroDeckPlugin
{
    /// <summary>
    /// 构造函数，从程序集自动提取插件名称和版本号。
    /// </summary>
    public MacroDeckPlugin()
    {
        var executingAssembly = GetType().Assembly;
        Name = executingAssembly.GetName().Name ?? throw new InvalidOperationException();
        Version = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            throw new InvalidOperationException();
    }

    /// <summary>插件名称</summary>
    internal virtual string Name { get; set; }

    /// <summary>插件版本</summary>
    internal virtual string Version { get; set; }

    /// <summary>插件作者</summary>
    internal virtual string Author { get; set; }

    /// <summary>
    /// 插件的动作列表。如果插件不包含任何动作，可以删除此属性。
    /// </summary>
    public List<PluginAction> Actions { get; set; } = new();

    /// <summary>插件图标</summary>
    internal virtual Image PluginIcon { get; set; } = Resources.Macro_Deck_2021;

    /// <summary>
    /// 插件是否可配置。为 true 时，包管理器中会显示配置按钮。
    /// 如果插件不可配置，可以删除此属性。
    /// </summary>
    public virtual bool CanConfigure { get; } = false;

    /// <summary>
    /// 当用户在包管理器中点击配置按钮时调用。
    /// 如果插件不可配置，可以删除此方法。
    /// </summary>
    public virtual void OpenConfigurator()
    {
    }

    /// <summary>
    /// 当插件被启用时调用。如果插件包含动作，应在此处初始化动作列表。
    /// </summary>
    public abstract void Enable();


    /// <summary>插件描述（已过时，即将移除）</summary>
    [Obsolete("Will be removed soon")] public virtual string Description { get; set; }

    /// <summary>插件图标（已过时，即将移除）</summary>
    [Obsolete("Will be removed soon")] public virtual Image Icon { get; }
}

/// <summary>
/// 插件动作抽象基类，所有插件动作必须继承此类。
/// 定义动作的触发、配置、显示名称和描述等核心功能。
/// </summary>
public abstract class PluginAction
{
    private ActionButton.ActionButton actionButton;

    /// <summary>
    /// 设置关联的动作按钮。当按钮被替换或删除时调用相应生命周期方法。
    /// </summary>
    /// <param name="actionButton">关联的动作按钮</param>
    internal void SetActionButton(ActionButton.ActionButton actionButton)
    {
        if (actionButton == null)
        {
            return;
        }

        // 如果已有按钮且不是同一个，先调用删除回调
        if (this.actionButton != null && this.actionButton.Equals(actionButton))
        {
            OnActionButtonDelete();
        }

        this.actionButton = actionButton;
        if (actionButton != null)
        {
            OnActionButtonLoaded();
        }
    }

    /// <summary>
    /// 获取包含此动作的动作按钮
    /// </summary>
    [JsonIgnore]
    public ActionButton.ActionButton ActionButton => actionButton;


    /// <summary>
    /// 可绑定到按钮状态的变量名
    /// </summary>
    public virtual string BindableVariable { get; set; }

    /// <summary>
    /// 当动作按钮被删除时调用
    /// </summary>
    public virtual void OnActionButtonDelete()
    {
    }

    /// <summary>
    /// 当动作按钮被加载时调用
    /// </summary>
    public virtual void OnActionButtonLoaded()
    {
    }

    /// <summary>动作名称</summary>
    public abstract string Name { get; }

    /// <summary>显示名称（已过时，请使用 ConfigurationSummary）</summary>
    [Obsolete("Use ConfigurationSummary instead")]
    public virtual string DisplayName { get; set; }

    /// <summary>动作描述</summary>
    public abstract string Description { get; }

    /// <summary>
    /// 当用户按下配置了此动作的按钮时调用。
    /// </summary>
    /// <param name="clientId">触发客户端的 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public abstract void Trigger(string clientId, ActionButton.ActionButton actionButton);

    /// <summary>动作的配置 JSON 字符串</summary>
    public string Configuration { get; set; }

    /// <summary>
    /// 配置摘要，在按钮编辑器中显示动作配置的简短描述。
    /// 例如："Example -> example value"
    /// </summary>
    public string ConfigurationSummary { get; set; }

    /// <summary>
    /// 动作是否可配置。为 true 时需要实现 ActionConfigControl。
    /// </summary>
    public virtual bool CanConfigure { get; } = false;

    /// <summary>
    /// 获取动作配置的用户控件。
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>配置控件实例，不可配置时返回 null</returns>
    public virtual ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return null;
    }


    /// <summary>
    /// 通过序列化创建插件动作的新实例。
    /// </summary>
    /// <param name="pluginAction">要复制的插件动作</param>
    /// <returns>新的插件动作实例</returns>
    public static PluginAction GetNewInstance(PluginAction pluginAction)
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; }
        };

        // 通过序列化创建动作按钮实例的副本
        return PluginManager.GetNewActionInstance(pluginAction);
    }
}
