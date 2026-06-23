using Newtonsoft.Json;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.ActionButton;

/// <summary>
/// 按钮标签类，表示显示在动作按钮上的文字标签。
/// 包含标签文本、位置、颜色、字体大小和字体族等属性，
/// 并自动生成不同尺寸的标签图像的 Base64 编码。
/// </summary>
public class ButtonLabel
{
    /// <summary>
    /// 标签内容变化事件
    /// </summary>
    public event EventHandler LabelBase64Changed;

    private string _labelBase64 = "";
    private string _labelHex128_64Base64 = "";

    /// <summary>
    /// 标签图像的 Base64 编码字符串。
    /// 设置时自动更新缩略图版本并触发变化事件。
    /// </summary>
    public string LabelBase64
    {
        get => _labelBase64;
        set
        {
            _labelBase64 = value;
            UpdateLabelHex128_64();
            LabelBase64Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 标签文本内容
    /// </summary>
    public string LabelText = "";

    /// <summary>
    /// 标签在按钮上的显示位置
    /// </summary>
    public ButtonLabelPosition LabelPosition = ButtonLabelPosition.BOTTOM;

    /// <summary>
    /// 标签文字颜色
    /// </summary>
    public Color LabelColor = Color.White;

    /// <summary>
    /// 标签字体大小
    /// </summary>
    public float Size = 6;

    /// <summary>
    /// 标签字体族名称
    /// </summary>
    public string FontFamily = "Impact";

    /// <summary>
    /// 缩放到 128x64 像素的标签图像 Base64 编码。
    /// 用于在小尺寸设备上显示标签，按需生成并缓存。
    /// </summary>
    [JsonIgnore]
    public string LabelHex128_64Base64
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_labelHex128_64Base64))
            {
                UpdateLabelHex128_64();
            }

            return _labelHex128_64Base64;
        }
    }

    /// <summary>
    /// 根据标签位置生成 128x64 像素的缩略图 Base64 编码。
    /// 将标签图像按照指定的对齐方式缩放绘制到目标尺寸。
    /// </summary>
    private void UpdateLabelHex128_64()
    {
        // 将标签位置枚举映射到内容对齐方式
        var contentAlignment = LabelPosition switch
        {
            ButtonLabelPosition.TOP => ContentAlignment.TopCenter,
            ButtonLabelPosition.BOTTOM => ContentAlignment.BottomCenter,
            _ => ContentAlignment.MiddleCenter
        };
        using var labelImage = Base64.GetImageFromBase64(_labelBase64);
        _labelHex128_64Base64 = Base64.GetBase64ByteArray((Bitmap)labelImage,
            new Size(128, 64),
            contentAlignment);
    }
}

/// <summary>
/// 按钮标签位置枚举
/// </summary>
public enum ButtonLabelPosition
{
    /// <summary>顶部居中</summary>
    TOP,
    /// <summary>居中</summary>
    CENTER,
    /// <summary>底部居中</summary>
    BOTTOM
}
