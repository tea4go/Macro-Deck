using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// Macro Deck 扩展接口，所有扩展类型（插件、图标包）必须实现此接口。
/// 提供统一的扩展类型标识、显示名称、扩展对象引用和可配置性等属性。
/// </summary>
public interface IMacroDeckExtension
{
    /// <summary>扩展类型（插件或图标包）</summary>
    public ExtensionType ExtensionType { get; }

    /// <summary>扩展类型的显示名称</summary>
    public string ExtensionTypeDisplayName { get; }

    /// <summary>扩展对象实例（MacroDeckPlugin 或 IconPack）</summary>
    public object ExtensionObject { get; }

    /// <summary>是否可配置</summary>
    public bool Configurable { get; }

    /// <summary>卸载扩展</summary>
    public void Uninstall();
}
