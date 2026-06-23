using Newtonsoft.Json;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 动作按钮剪贴板，支持复制和粘贴动作按钮。
/// 通过 JSON 序列化/反序列化实现深拷贝，确保粘贴时创建独立的按钮实例。
/// </summary>
public class Clipboard
{
    /// <summary>剪贴板中的源动作按钮</summary>
    private static ActionButton.ActionButton _actionButtonSource;

    /// <summary>
    /// JSON 序列化设置，包含类型名称处理和错误处理
    /// </summary>
    private static JsonSerializerSettings jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,

        Error = (sender, args) => { args.ErrorContext.Handled = true; }
    };

    /// <summary>
    /// 复制动作按钮到剪贴板
    /// </summary>
    /// <param name="actionButton">要复制的动作按钮</param>
    public static void CopyActionButton(ActionButton.ActionButton actionButton)
    {
        _actionButtonSource = actionButton;
    }

    /// <summary>
    /// 从剪贴板获取动作按钮的深拷贝。
    /// 通过 JSON 序列化再反序列化创建完全独立的副本。
    /// 如果源按钮为 null 或已释放，则返回 null。
    /// </summary>
    /// <returns>动作按钮的深拷贝，或 null</returns>
    public static ActionButton.ActionButton GetActionButtonCopy()
    {
        if (_actionButtonSource == null)
        {
            return null;
        }

        if (_actionButtonSource.IsDisposed)
        {
            return null;
        }

        // 通过 JSON 序列化/反序列化创建深拷贝
        return JsonConvert.DeserializeObject<ActionButton.ActionButton>(
            JsonConvert.SerializeObject(_actionButtonSource, jsonSerializerSettings),
            jsonSerializerSettings);
    }
}
