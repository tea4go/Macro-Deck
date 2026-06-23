using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.ActionButton;
// Don't change because of backwards compatibility!

/// <summary>
/// 设置动作按钮为开启状态动作。
/// 如果按钮已经是开启状态则不执行任何操作。
/// </summary>
public class ActionButtonSetStateOnAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSetActionButtonStateOn;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSetActionButtonStateOnDescription;

    /// <summary>是否可配置（此动作不可配置）</summary>
    public override bool CanConfigure => false;

    /// <summary>
    /// 触发设置开启状态动作。如果按钮已经是开启状态则直接返回。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">目标动作按钮</param>
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        // 如果已经是开启状态，无需重复设置
        if (actionButton.State)
        {
            return;
        }

        MacroDeckServer.SetState(actionButton, true);
    }
}
