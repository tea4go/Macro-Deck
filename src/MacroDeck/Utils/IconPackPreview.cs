using System.Drawing.Drawing2D;
using SuchByte.MacroDeck.Icons;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 图标包预览图生成器，从图标包中取前 4 个图标生成 2x2 网格预览图。
/// 用于扩展管理器中显示图标包的缩略预览。
/// </summary>
public static class IconPackPreview
{
    /// <summary>
    /// 为图标包生成 80x80 像素的预览图。
    /// 取图标包前 4 个图标排列成 2x2 网格，背景为深灰色。
    /// </summary>
    /// <param name="iconPack">目标图标包</param>
    /// <returns>预览位图</returns>
    public static Image GeneratePreviewImage(IconPack iconPack)
    {
        var totalSize = 80;
        var bitmap = new Bitmap(totalSize, totalSize);

        var padding = 2;
        var iconSize = 80 / 2;

        int row = 0, column = 0;

        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.FromArgb(32, 32, 32));
        }

        using var canvas = Graphics.FromImage(bitmap);
        canvas.InterpolationMode = InterpolationMode.Bicubic;

        // 取前 4 个图标排列成 2x2 网格
        foreach (var icon in iconPack.Icons.Take(4))
        {
            try
            {
                var iconRectangle = new Rectangle
                {
                    Height = iconSize - padding,
                    Width = iconSize - padding,
                    X = column * (iconSize + column * padding),
                    Y = row * (iconSize + row * padding)
                };
                using (var iconImage = icon.IconImage)
                {
                    canvas.DrawImage(iconImage, iconRectangle);
                }

                column++;
                if (column >= 2)
                {
                    column = 0;
                    row++;
                }
            }
            catch
            {
            }
        }

        canvas.Save();

        return bitmap;
    }
}
