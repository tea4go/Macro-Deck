using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;

namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// 插件扩展类，将 MacroDeckPlugin 包装为 IMacroDeckExtension 接口实现。
/// 根据插件的 CanConfigure 属性决定是否可配置。
/// </summary>
public class PluginExtension : IMacroDeckExtension
{
    /// <summary>扩展类型：插件</summary>
    public ExtensionType ExtensionType => ExtensionType.Plugin;

    /// <summary>扩展类型显示名称</summary>
    public string ExtensionTypeDisplayName => LanguageManager.Strings.Plugin;

    /// <summary>插件对象实例</summary>
    public object ExtensionObject { get; set; }

    /// <summary>是否可配置，取决于插件的 CanConfigure 属性</summary>
    public bool Configurable => ExtensionObject != null && (ExtensionObject as MacroDeckPlugin).CanConfigure;

    /// <summary>
    /// 构造函数，包装指定的插件实例
    /// </summary>
    /// <param name="macroDeckPlugin">要包装的插件</param>
    public PluginExtension(MacroDeckPlugin macroDeckPlugin)
    {
        ExtensionObject = macroDeckPlugin;
    }

    /// <summary>
    /// 卸载插件（当前为空实现）
    /// </summary>
    public void Uninstall()
    {
    }
}
