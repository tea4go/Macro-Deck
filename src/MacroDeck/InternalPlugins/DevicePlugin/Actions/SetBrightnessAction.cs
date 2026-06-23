using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Models;
using SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Views;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;

namespace SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Actions;

/// <summary>
/// 设置亮度动作，调整已连接设备的屏幕亮度。
/// 仅对 Android 和 iOS 设备有效，本地软件触发时直接返回。
/// </summary>
public class SetBrightnessAction : PluginAction
{
    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionSetBrightness;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionSetBrightnessDescription;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>
    /// 触发设置亮度动作。
    /// 仅对已连接的 Android 或 iOS 设备有效：
    /// 1. 如果是本地软件触发（clientId 为空或 "-1"），直接返回
    /// 2. 根据配置中的 ClientId 或触发客户端 ID 获取设备
    /// 3. 检查设备是否可用且为 Android 或 iOS 类型
    /// 4. 设置设备亮度并保存配置，然后发送配置更新到客户端
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        var configModel = SetBrightnessActionConfigModel.Deserialize(Configuration);

        // 本地软件触发时直接返回（本地软件无亮度概念）
        if (clientId == "" || clientId == "-1")
        {
            return;
        }

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

        // 检查设备是否可用且为 Android 或 iOS 类型
        if (macroDeckDevice == null ||
            !macroDeckDevice.Available ||
            (macroDeckDevice.DeviceType != DeviceType.Android && macroDeckDevice.DeviceType != DeviceType.iOS))
        {
            return;
        }

        // 设置亮度并保存配置
        macroDeckDevice.Configuration.Brightness = configModel.Brightness;

        DeviceManager.SaveKnownDevices();

        // 发送配置更新到客户端
        var macroDeckClient = MacroDeckServer.GetMacroDeckClient(macroDeckDevice.ClientId);
        macroDeckClient?.DeviceMessage.SendConfiguration(macroDeckClient);
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>设置亮度配置视图</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SetBrightnessActionConfigView(this);
    }
}
