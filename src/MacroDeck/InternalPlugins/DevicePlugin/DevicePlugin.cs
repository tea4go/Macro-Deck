using SuchByte.MacroDeck.InternalPlugins.DevicePlugin.Actions;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.InternalPlugins.DevicePlugin;

/// <summary>
/// 设备内部插件，提供设备相关动作。
/// 包含切换配置文件和设置亮度等动作。
/// </summary>
public class DevicePlugin : MacroDeckPlugin
{
    /// <summary>插件名称</summary>
    internal override string Name => "Device";

    /// <summary>插件作者</summary>
    internal override string Author => "Macro Deck";

    /// <summary>插件图标</summary>
    internal override Image PluginIcon => Resources.device_manager;

    /// <summary>
    /// 启用插件，注册设备相关动作
    /// </summary>
    public override void Enable()
    {
        Actions = new List<PluginAction>
        {
            new SetProfileAction(),
            new SetBrightnessAction()
        };
    }
}
