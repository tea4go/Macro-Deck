using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using SuchByte.MacroDeck.Device;
using Serilog;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.JSON;

namespace SuchByte.MacroDeck.Server.DeviceMessage;

/// <summary>
/// 软件客户端消息处理器，实现 IDeviceMessage 接口。
/// 为软件客户端（Web、Android、iOS 等）生成和发送 JSON 格式的消息，
/// 包括按钮数据、配置信息和状态更新。
/// </summary>
public class SoftwareClientMessage : IDeviceMessage
{
    private static readonly ILogger Logger = Log.ForContext(typeof(SoftwareClientMessage));

    /// <summary>
    /// 客户端连接成功后发送配置和所有按钮数据。
    /// 同时检查并更新设备类型信息。
    /// </summary>
    /// <param name="macroDeckClient">已连接的客户端</param>
    public void Connected(MacroDeckClient macroDeckClient)
    {
        SendConfiguration(macroDeckClient);
        SendAllButtons(macroDeckClient);

        // 如果设备类型发生变化，更新并保存设备信息
        if (macroDeckClient.DeviceType != DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).DeviceType)
        {
            DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).DeviceType = macroDeckClient.DeviceType;
            DeviceManager.SaveKnownDevices();
        }
    }

    /// <summary>
    /// 并行发送当前文件夹的所有按钮数据。
    /// 使用 ConcurrentBag 和 Parallel.ForEach 实现线程安全的并行处理，
    /// 每个按钮根据其开/关状态选择对应的图标、标签和背景色。
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    public void SendAllButtons(MacroDeckClient macroDeckClient)
    {
        if (macroDeckClient == null || macroDeckClient.Folder == null || macroDeckClient.Folder.ActionButtons == null)
        {
            return;
        }

        var buttons = new ConcurrentBag<JObject>();

        // 并行处理所有按钮，生成按钮数据对象
        Parallel.ForEach(macroDeckClient.Folder.ActionButtons,
            actionButton =>
            {
                var IconBase64 = "";
                var LabelBase64 = "";
                string BackgroundColorHex;

                // 根据按钮开/关状态选择对应的图标、标签和背景色
                if (!actionButton.State)
                {
                    // 关闭状态：使用 IconOff、LabelOff、BackColorOff
                    if (!string.IsNullOrWhiteSpace(actionButton.IconOff))
                    {
                        var icon = IconManager.GetIconByString(actionButton.IconOff);
                        if (icon != null)
                        {
                            IconBase64 = icon.IconBase64;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(actionButton.LabelOff.LabelText))
                    {
                        LabelBase64 = actionButton.LabelOff.LabelBase64 ?? "";
                    }

                    BackgroundColorHex
                        = $"#{actionButton.BackColorOff.R:X2}{actionButton.BackColorOff.G:X2}{actionButton.BackColorOff.B:X2}";
                }
                else
                {
                    // 开启状态：使用 IconOn、LabelOn、BackColorOn
                    if (!string.IsNullOrWhiteSpace(actionButton.IconOn))
                    {
                        var icon = IconManager.GetIconByString(actionButton.IconOn);
                        if (icon != null)
                        {
                            IconBase64 = icon.IconBase64;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(actionButton.LabelOn.LabelText))
                    {
                        LabelBase64 = actionButton.LabelOn.LabelBase64 ?? "";
                    }

                    BackgroundColorHex
                        = $"#{actionButton.BackColorOn.R:X2}{actionButton.BackColorOn.G:X2}{actionButton.BackColorOn.B:X2}";
                }

                var actionButtonObject = JObject.FromObject(new
                {
                    IconBase64,
                    actionButton.Position_X,
                    actionButton.Position_Y,
                    LabelBase64,
                    BackgroundColorHex
                });
                buttons.Add(actionButtonObject);
            });

        var buttonsObject = JObject.FromObject(new
        {
            Method = JsonMethod.GET_BUTTONS.ToString(),
            Buttons = buttons
        });

        MacroDeckServer.Send(macroDeckClient, buttonsObject);
    }

    /// <summary>
    /// 发送配置信息，包括网格行列数、按钮间距、圆角、背景色、
    /// 设备亮度、自动连接设置和屏幕唤醒方式。
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    public void SendConfiguration(MacroDeckClient macroDeckClient)
    {
        if (macroDeckClient == null)
        {
            return;
        }

        var configurationObject = JObject.FromObject(new
        {
            Method = JsonMethod.GET_CONFIG.ToString(),
            macroDeckClient.Profile.Rows,
            macroDeckClient.Profile.Columns,
            macroDeckClient.Profile.ButtonSpacing,
            macroDeckClient.Profile.ButtonRadius,
            macroDeckClient.Profile.ButtonBackground,
            DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).Configuration.Brightness,
            DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).Configuration.AutoConnect,
            WakeLock = Enum.GetName(typeof(WakeLockMethod),
                DeviceManager.GetMacroDeckDevice(macroDeckClient.ClientId).Configuration.WakeLockMethod),
            SupportButtonReleaseLongPress = true
        });
        Logger.Debug("{Configuration}", configurationObject.ToString());
        MacroDeckServer.Send(macroDeckClient, configurationObject);
    }

    /// <summary>
    /// 更新单个按钮的数据。根据按钮的开/关状态选择对应的图标、标签和背景色，
    /// 生成按钮更新消息并发送到客户端。
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="actionButton">要更新的按钮</param>
    public void UpdateButton(MacroDeckClient macroDeckClient, ActionButton.ActionButton actionButton)
    {
        if (macroDeckClient.Folder == null || !macroDeckClient.Folder.ActionButtons.Contains(actionButton))
        {
            return;
        }

        var IconBase64 = "";
        var LabelBase64 = "";
        string BackgroundColorHex;

        // 根据按钮开/关状态选择对应的图标、标签和背景色
        if (!actionButton.State)
        {
            // 关闭状态
            if (!string.IsNullOrWhiteSpace(actionButton.IconOff))
            {
                var icon = IconManager.GetIconByString(actionButton.IconOff);
                if (icon != null)
                {
                    IconBase64 = icon.IconBase64;
                }
            }

            if (!string.IsNullOrWhiteSpace(actionButton.LabelOff.LabelText))
            {
                LabelBase64 = actionButton.LabelOff.LabelBase64 ?? "";
            }

            BackgroundColorHex
                = $"#{actionButton.BackColorOff.R:X2}{actionButton.BackColorOff.G:X2}{actionButton.BackColorOff.B:X2}";
        }
        else
        {
            // 开启状态
            if (!string.IsNullOrWhiteSpace(actionButton.IconOn))
            {
                var icon = IconManager.GetIconByString(actionButton.IconOn);
                if (icon != null)
                {
                    IconBase64 = icon.IconBase64;
                }
            }

            if (!string.IsNullOrWhiteSpace(actionButton.LabelOn.LabelText))
            {
                LabelBase64 = actionButton.LabelOn.LabelBase64 ?? "";
            }

            BackgroundColorHex
                = $"#{actionButton.BackColorOn.R:X2}{actionButton.BackColorOn.G:X2}{actionButton.BackColorOn.B:X2}";
        }

        var actionButtonObject = JObject.FromObject(new
        {
            IconBase64,
            actionButton.Position_X,
            actionButton.Position_Y,
            LabelBase64,
            BackgroundColorHex
        });

        var updateObject = JObject.FromObject(new
        {
            Method = JsonMethod.UPDATE_BUTTON.ToString(),
            Buttons = new List<JObject>
            {
                actionButtonObject
            }
        });

        MacroDeckServer.Send(macroDeckClient, updateObject);
    }
}
