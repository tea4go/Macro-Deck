using System.Runtime.CompilerServices;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 布局助手，为 Designer 硬编码 Size 的 WinForms 对话框提供字体自适应能力。
///
/// 使用模式：在对话框的 OnLoad 中（base.OnLoad 已经调用 FontManager.Apply 之后）调用
/// AdjustLabelsAndButtons + AdjustFormSize，即可让控件尺寸和窗体大小适配当前字体。
///
/// 对于布局复杂、含嵌套容器的对话框，建议覆盖 OnLoad 自行实现精确的布局重算逻辑。
/// </summary>
public static class LayoutHelper
{
    /// <summary>
    /// 计算当前字体下一行文字的最小高度（含 1px 余量）。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetTextHeight(Control control) =>
        TextRenderer.MeasureText("Ay", control.Font).Height + 1;

    /// <summary>
    /// 为 Label 控件计算合适的最小高度（文字高度 + 4px padding）。
    /// 仅在需要增大时才调整，不会缩小。
    /// </summary>
    public static void AdjustLabelHeight(Label label)
    {
        var min = GetTextHeight(label) + 4;
        if (label.Height < min) label.Height = min;
    }

    /// <summary>
    /// 为 RadioButton 控件计算合适的最小高度。
    /// </summary>
    public static void AdjustRadioHeight(RadioButton radio)
    {
        var min = GetTextHeight(radio) + 4;
        if (radio.Height < min) radio.Height = min;
    }

    /// <summary>
    /// 为 CheckBox 控件计算合适的最小高度。
    /// </summary>
    public static void AdjustCheckBoxHeight(CheckBox checkBox)
    {
        var min = GetTextHeight(checkBox) + 4;
        if (checkBox.Height < min) checkBox.Height = min;
    }

    /// <summary>
    /// 遍历窗体所有控件，自动调整 Label、RadioButton、CheckBox 的高度。
    /// 跳过 Tag 为 "no-font" 的控件和已设置 AutoSize 的控件。
    /// </summary>
    public static void AdjustAllLabelHeights(Control root)
    {
        AdjustRecursive(root);

        static void AdjustRecursive(Control control)
        {
            if (control.Tag as string == "no-font") return;

            switch (control)
            {
                case Label label when !label.AutoSize:
                    AdjustLabelHeight(label);
                    break;
                case RadioButton radio when !radio.AutoSize:
                    AdjustRadioHeight(radio);
                    break;
                case CheckBox checkBox when !checkBox.AutoSize:
                    AdjustCheckBoxHeight(checkBox);
                    break;
            }

            foreach (Control child in control.Controls)
                AdjustRecursive(child);
        }
    }

    /// <summary>
    /// 根据所有控件的实际位置调整窗体的 ClientSize，确保没有控件超出边界。
    /// 会取所有控件的 Right/Bottom 最大值加上边距（默认 12px），
    /// 且不会缩小到小于设计时的原始尺寸。
    /// </summary>
    /// <param name="form">要调整的窗体</param>
    /// <param name="originalClientSize">设计时的原始 ClientSize，作为最小尺寸</param>
    /// <param name="margin">右边距和下边距（默认 12px）</param>
    public static void AdjustFormToFitControls(Form form, Size originalClientSize, int margin = 12)
    {
        var maxRight = originalClientSize.Width - margin;
        var maxBottom = originalClientSize.Height - margin;

        foreach (Control control in form.Controls)
        {
            if (!control.Visible) continue;
            maxRight = Math.Max(maxRight, control.Right);
            maxBottom = Math.Max(maxBottom, control.Bottom);
        }

        var newWidth = Math.Max(originalClientSize.Width, maxRight + margin);
        var newHeight = Math.Max(originalClientSize.Height, maxBottom + margin);
        form.ClientSize = new Size(newWidth, newHeight);
    }
}
