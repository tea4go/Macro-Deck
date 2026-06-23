using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.Language;

namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// 图标包扩展类，将 IconPack 包装为 IMacroDeckExtension 接口实现。
/// 图标包不支持配置和卸载操作。
/// </summary>
public class IconPackExtension : IMacroDeckExtension
{
    /// <summary>扩展类型：图标包</summary>
    public ExtensionType ExtensionType => ExtensionType.IconPack;

    /// <summary>扩展类型显示名称</summary>
    public string ExtensionTypeDisplayName => LanguageManager.Strings.IconPack;

    /// <summary>图标包对象实例</summary>
    public object ExtensionObject { get; set; }

    /// <summary>图标包不可配置</summary>
    public bool Configurable => false;

    /// <summary>
    /// 构造函数，包装指定的图标包实例
    /// </summary>
    /// <param name="iconPack">要包装的图标包</param>
    public IconPackExtension(IconPack iconPack)
    {
        ExtensionObject = iconPack;
    }

    /// <summary>
    /// 卸载图标包（当前为空实现）
    /// </summary>
    public void Uninstall()
    {
    }
}
