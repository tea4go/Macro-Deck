using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 位图合成工具类，将多个位图合成为单个图像。
/// 用于动作按钮的背景和图标合成。
/// </summary>
public class CombineBitmaps
{
    /// <summary>
    /// 将所有位图叠加合成为一张 350x350 的图像。
    /// 所有位图从左上角 (0,0) 开始叠加绘制。
    /// </summary>
    /// <param name="bitmaps">要合成的位图列表</param>
    /// <returns>合成后的位图</returns>
    public static Bitmap CombineAll(List<Bitmap> bitmaps)
    {
        var combined = new Bitmap(350, 350);

        using var g = Graphics.FromImage(combined);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        foreach (var bitmap in bitmaps)
        {
            g.DrawImage(bitmap, Point.Empty);
        }

        return combined;
    }

    /// <summary>
    /// 将背景位图和图标位图合成为一张 350x350 的图像。
    /// 背景铺满整个画布，图标居中绘制在背景之上。
    /// 如果任一参数为 null，则使用透明位图替代。
    /// </summary>
    /// <param name="backgroundBitmap">背景位图</param>
    /// <param name="iconBitmap">图标位图</param>
    /// <returns>合成后的位图</returns>
    public static Bitmap Combine(Bitmap backgroundBitmap, Bitmap iconBitmap)
    {
        var px = 350;
        if (backgroundBitmap == null)
        {
            backgroundBitmap = new Bitmap(px, px);
        }

        if (iconBitmap == null)
        {
            iconBitmap = new Bitmap(px, px);
        }

        using Image background =
            new Bitmap(backgroundBitmap.Width, backgroundBitmap.Height, PixelFormat.Format32bppArgb);
        using Image icon = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(background))
        {
            g.Clear(Color.Transparent);
        }

        using (var g = Graphics.FromImage(icon))
        {
            g.Clear(Color.Transparent);
        }

        var bitmap = new Bitmap(px, px);
        using (var canvas = Graphics.FromImage(bitmap))
        {
            canvas.InterpolationMode = InterpolationMode.Bicubic;
            // 绘制背景，铺满画布
            canvas.DrawImage(background,
                new Rectangle(0,
                    0,
                    background.Width,
                    background.Height),
                new Rectangle(0,
                    0,
                    background.Width,
                    background.Height),
                GraphicsUnit.Pixel);
            // 居中绘制图标
            canvas.DrawImage(icon,
                bitmap.Width / 2 - icon.Width / 2,
                bitmap.Height / 2 - icon.Height / 2);

            canvas.Save();
        }

        return bitmap;
    }
}
