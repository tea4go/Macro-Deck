using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin.Enums;
using SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin.Models;
using SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin.Views;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;

namespace SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin.Actions;

/// <summary>
/// 设置动作按钮背景色动作。
/// 支持两种设置方式：固定颜色和随机颜色。
/// 根据按钮当前状态设置对应的背景色（开状态设置 BackColorOn，关状态设置 BackColorOff）。
/// </summary>
public class ActionButtonSetBackgroundColorAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSetBackgroundColor;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSetBackgroundColorDescription;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>随机数生成器，用于生成随机颜色</summary>
    private Random random;

    /// <summary>
    /// 触发设置背景色动作。
    /// 根据配置的方法类型选择颜色：
    /// - Fixed：使用配置中指定的固定颜色
    /// - Random：生成随机 RGB 颜色
    /// 然后根据按钮当前状态设置对应的背景色并保存配置文件。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">目标动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        var configModel = ActionButtonSetBackgroundColorActionConfigModel.Deserialize(Configuration);
        var color = Color.FromArgb(35, 35, 35);
        switch (configModel.Method)
        {
            case SetBackgroundColorMethod.Fixed:
                // 固定颜色：使用配置中指定的颜色
                color = configModel.Color;
                break;
            case SetBackgroundColorMethod.Random:
                // 随机颜色：生成随机 RGB 值
                if (random == null)
                {
                    random = new Random();
                }

                color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                break;
        }

        // 根据按钮状态设置对应的背景色
        if (actionButton.State)
        {
            actionButton.BackColorOn = color;
        }
        else
        {
            actionButton.BackColorOff = color;
        }

        ProfileManager.Save();
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>设置背景色配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new ActionButtonSetBackgroundColorActionConfigView(this);
    }
}
