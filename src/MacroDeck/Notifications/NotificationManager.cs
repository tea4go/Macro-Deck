using SuchByte.MacroDeck.Models;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.Notifications;

/// <summary>
/// 通知事件参数，携带通知模型
/// </summary>
public class NotificationEventArgs : EventArgs
{
    /// <summary>通知模型</summary>
    public NotificationModel Notification { get; set; }
}

/// <summary>
/// 通知移除事件参数，携带被移除通知的 ID
/// </summary>
public class NotificationRemovedEventArgs : EventArgs
{
    /// <summary>被移除通知的 ID</summary>
    public string Id { get; set; }
}

/// <summary>
/// 通知管理器，负责系统通知的创建、发送和移除。
/// 每个发送者最多保留 5 条通知，避免通知过多。
/// 支持插件通知和系统通知，可选显示系统托盘气泡提示。
/// </summary>
public class NotificationManager
{
    /// <summary>新通知创建事件</summary>
    public static EventHandler<NotificationEventArgs> OnNotification;

    /// <summary>通知移除事件</summary>
    public static EventHandler<NotificationRemovedEventArgs> OnNotificationRemoved;

    /// <summary>通知列表</summary>
    private static List<NotificationModel> _notifications = new();

    /// <summary>获取所有通知列表</summary>
    internal static List<NotificationModel> Notifications => _notifications;

    /// <summary>
    /// 根据 ID 获取通知
    /// </summary>
    /// <param name="id">通知 ID</param>
    /// <returns>通知模型，未找到返回 null</returns>
    public static NotificationModel GetNotification(string id)
    {
        return _notifications.Find(x => x.Id == id);
    }

    /// <summary>
    /// 发送插件通知。创建通知模型并添加到通知列表。
    /// </summary>
    /// <param name="macroDeckPlugin">发送通知的插件</param>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知消息</param>
    /// <param name="showBalloonTip">是否显示系统托盘气泡提示</param>
    /// <param name="controls">附加的自定义控件列表</param>
    /// <returns>通知 ID</returns>
    public static string Notify(MacroDeckPlugin macroDeckPlugin,
        string title,
        string message,
        bool showBalloonTip = false,
        List<Control> controls = null)
    {
        var notificationModel = new NotificationModel
        {
            SenderName = macroDeckPlugin.Name,
            Title = title,
            Message = message,
            AdditionalControls = controls,
            Icon = (Bitmap)macroDeckPlugin.PluginIcon
        };

        Notify(notificationModel, showBalloonTip);

        return notificationModel.Id;
    }

    /// <summary>
    /// 根据通知模型移除通知
    /// </summary>
    /// <param name="notificationModel">要移除的通知模型</param>
    public static void RemoveNotification(NotificationModel notificationModel)
    {
        if (notificationModel == null || !_notifications.Contains(notificationModel))
        {
            return;
        }

        _notifications.Remove(notificationModel);

        OnNotificationRemoved?.Invoke(null, new NotificationRemovedEventArgs { Id = notificationModel.Id });
    }

    /// <summary>
    /// 根据通知 ID 移除通知
    /// </summary>
    /// <param name="id">要移除的通知 ID</param>
    public static void RemoveNotification(string id)
    {
        RemoveNotification(_notifications.Find(x => x.Id == id));
    }

    /// <summary>
    /// 发送系统通知（发送者为 "Macro Deck"）。
    /// </summary>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知消息</param>
    /// <param name="showBalloonTip">是否显示系统托盘气泡提示</param>
    /// <param name="controls">附加的自定义控件列表</param>
    /// <param name="icon">通知图标，为 null 时使用默认图标</param>
    /// <returns>通知 ID</returns>
    internal static string SystemNotification(string title,
        string message,
        bool showBalloonTip = false,
        List<Control> controls = null,
        Bitmap icon = null)
    {
        var notificationModel = new NotificationModel
        {
            SenderName = "Macro Deck",
            Title = title,
            Message = message,
            AdditionalControls = controls,
            Icon = icon == null ? Resources.Macro_Deck_2021 : icon
        };

        Notify(notificationModel, showBalloonTip);

        return notificationModel.Id;
    }

    /// <summary>
    /// 内部通知发送方法。
    /// 检查通知是否已存在（防止重复），以及同一发送者是否已达到 5 条上限。
    /// 通过 UI 同步上下文触发通知事件，确保在 UI 线程上执行。
    /// </summary>
    /// <param name="notificationModel">通知模型</param>
    /// <param name="showBalloonTip">是否显示系统托盘气泡提示</param>
    private static void Notify(NotificationModel notificationModel, bool showBalloonTip)
    {
        // 防止重复通知
        if (_notifications.Find(x => x.Id == notificationModel.Id) != null)
        {
            return;
        }

        // 每个发送者最多保留 5 条通知
        if (_notifications.FindAll(x => x.SenderName == notificationModel.SenderName).Count >= 5)
        {
            return;
        }

        _notifications.Add(notificationModel);

        // 通过 UI 同步上下文在 UI 线程上触发通知事件
        MacroDeck.SyncContext?.Send(o =>
            {
                OnNotification?.Invoke(null, new NotificationEventArgs { Notification = notificationModel });
            },
            null);

        // 可选显示系统托盘气泡提示
        if (showBalloonTip)
        {
            MacroDeck.ShowBalloonTip($"{notificationModel.SenderName}: {notificationModel.Title}",
                notificationModel.Message);
        }
    }
}
