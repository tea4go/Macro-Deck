using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Models;
using SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Views;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;

namespace SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Actions;

/// <summary>
/// 设置配置文件动作，将设备切换到指定的配置文件。
/// 支持本地 Macro Deck 软件和已连接的远程设备两种触发来源。
/// </summary>
public class SetProfileAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSetProfile;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSetProfileDescription;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>
    /// 触发设置配置文件动作。
    /// 根据 clientId 区分触发来源：
    /// - 空字符串或 "-1"：Macro Deck 软件本身，切换主窗口的配置文件
    /// - 其他值：已连接的远程设备，切换该设备的配置文件
    /// 如果配置中指定了 ClientId，则使用配置中的 ClientId，否则使用触发客户端的 ID。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        var configModel = SetProfileActionConfigModel.Deserialize(Configuration);

        var profile = ProfileManager.FindProfileById(configModel.ProfileId);
        switch (clientId)
        {
            // ClientID -1 or "" = Macro Deck software itself
            case "":
            case "-1":
                // 本地软件：切换主窗口 DeckView 的配置文件
                if (MacroDeck.MainWindow != null && MacroDeck.MainWindow.DeckView != null)
                {
                    MacroDeck.MainWindow.DeckView.SetProfile(profile);
                }

                break;
            // ClientId != -1 = Connected device
            default:
                // 远程设备：切换指定设备的配置文件
                MacroDeckDevice macroDeckDevice;
                if (string.IsNullOrWhiteSpace(configModel.ClientId))
                {
                    // 未指定 ClientId 时使用触发客户端的 ID
                    macroDeckDevice = DeviceManager.GetMacroDeckDevice(clientId);
                }
                else
                {
                    // 使用配置中指定的 ClientId
                    macroDeckDevice = DeviceManager.GetMacroDeckDevice(configModel.ClientId);
                }

                if (macroDeckDevice == null || profile == null)
                {
                    return;
                }

                DeviceManager.SetProfile(macroDeckDevice, profile);
                break;
        }
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>设置配置文件配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SetProfileActionConfigView(this);
    }
}
