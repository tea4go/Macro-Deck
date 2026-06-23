namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 通知模型类，表示系统或插件发送给用户的通知消息。
/// 通知可包含自定义控件和图标，显示在主窗口的通知区域。
/// </summary>
public class NotificationModel
{
    /// <summary>通知发送者名称</summary>
    public string SenderName { get; set; }

    /// <summary>通知标题</summary>
    public string Title { get; set; }

    /// <summary>通知消息内容</summary>
    public string Message { get; set; }

    /// <summary>通知唯一标识</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>通知创建时间戳（Unix 秒）</summary>
    public long Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

    /// <summary>附加的自定义控件列表，显示在通知中</summary>
    public List<Control> AdditionalControls { get; set; }

    /// <summary>通知图标</summary>
    public Bitmap Icon { get; set; }
}
