using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin.Actions;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin;
// Don't change because of backwards compatibility!

/// <summary>
/// 动作按钮内部插件，提供动作按钮状态控制相关动作。
/// 包含切换状态、设置开/关状态和设置背景色等动作。
/// </summary>
public class ActionButtonPlugin : MacroDeckPlugin
{
    /// <summary>插件名称</summary>
    internal override string Name => LanguageManager.Strings.PluginActionButton;

    /// <summary>插件作者</summary>
    internal override string Author => "Macro Deck";

    /// <summary>
    /// 启用插件，注册所有动作按钮相关动作
    /// </summary>
    public override void Enable()
    {
        Actions =
        [
            new ActionButtonToggleStateAction(),
            new ActionButtonSetStateOffAction(),
            new ActionButtonSetStateOnAction(),
            new ActionButtonSetBackgroundColorAction()
        ];
    }
}
