using System.Drawing.Drawing2D;
using SuchByte.MacroDeck.ActionButton;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 标签生成器，在动作按钮图像上绘制文本标签。
/// 支持顶部、居中、底部三种标签位置，并带有描边效果以提高可读性。
/// </summary>
public class LabelGenerator
{
    /// <summary>
    /// 在图像上绘制文本标签，带描边效果。
    /// 根据指定的标签位置对齐文本，使用黑色描边和指定颜色填充。
    /// </summary>
    /// <param name="img">目标图像（直接在此图像上绘制）</param>
    /// <param name="text">要绘制的文本</param>
    /// <param name="buttonLabelPosition">标签位置（顶部、居中、底部）</param>
    /// <param name="font">文本字体</param>
    /// <param name="textColor">文本填充颜色</param>
    /// <param name="shadowColor">描边颜色（未使用，描边固定为黑色）</param>
    /// <param name="shadowOffset">阴影偏移（未使用）</param>
    /// <returns>绘制了标签的图像</returns>
    public static Image GetLabel(Image img,
        string text,
        ButtonLabelPosition buttonLabelPosition,
        Font font,
        Color textColor,
        Color shadowColor,
        SizeF shadowOffset)
    {
        if (img == null)
        {
            return img;
        }

        var g = Graphics.FromImage(img);

        var sf = new StringFormat
        {
            Alignment = StringAlignment.Center
        };

        // 根据标签位置设置垂直对齐方式
        if (buttonLabelPosition == ButtonLabelPosition.TOP)
        {
            sf.LineAlignment = StringAlignment.Near;
        }
        else if (buttonLabelPosition == ButtonLabelPosition.CENTER)
        {
            sf.LineAlignment = StringAlignment.Center;
        }
        else
        {
            sf.LineAlignment = StringAlignment.Far;
        }

        // 使用黑色描边提高文本可读性
        var p = new Pen(Color.Black, 2)
        {
            LineJoin = LineJoin.Round
        };

        var b = new SolidBrush(textColor);

        var r = new Rectangle(2, 2, img.Width - 2, img.Height - 2);

        var gp = new GraphicsPath();

        gp.AddString(text, font.FontFamily, (int)font.Style, font.Size * 5, r, sf);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // 先绘制描边，再填充文本
        g.DrawPath(p, gp);
        g.FillPath(b, gp);

        gp.Dispose();
        b.Dispose();
        b.Dispose();
        font.Dispose();
        sf.Dispose();
        g.Dispose();

        return img;
    }
}
