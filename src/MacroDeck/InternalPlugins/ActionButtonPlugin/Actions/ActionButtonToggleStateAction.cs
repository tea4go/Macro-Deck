using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.ActionButton;
// Don't change because of backwards compatibility!

/// <summary>
/// 切换动作按钮状态动作，将按钮的开关状态取反。
/// </summary>
public class ActionButtonToggleStateAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionToggleActionButtonState;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionToggleActionButtonStateDescription;

    /// <summary>
    /// 触发切换状态动作，将按钮状态取反（开变关，关变开）
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">目标动作按钮</param>
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        MacroDeckServer.SetState(actionButton, !actionButton.State);
    }
}
