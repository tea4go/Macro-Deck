using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.ActionButton;
// Don't change because of backwards compatibility!

/// <summary>
/// 设置动作按钮为关闭状态动作。
/// 如果按钮已经是关闭状态则不执行任何操作。
/// </summary>
public class ActionButtonSetStateOffAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSetActionButtonStateOff;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSetActionButtonStateOffDescription;

    /// <summary>
    /// 触发设置关闭状态动作。如果按钮已经是关闭状态则直接返回。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">目标动作按钮</param>
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        // 如果已经是关闭状态，无需重复设置
        if (actionButton.State == false)
        {
            return;
        }

        MacroDeckServer.SetState(actionButton, false);
    }
}
