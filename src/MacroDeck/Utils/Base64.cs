using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// Base64 编解码工具类，提供图像与 Base64 字符串之间的转换功能。
/// 还包含将位图转换为客户端显示用的二值化 Base64 编码的特殊方法。
/// </summary>
public class Base64
{
    /// <summary>
    /// 将位图按指定尺寸裁剪并转换为二值化（黑白）Base64 编码字节数组。
    /// 图像先缩放到指定宽度（正方形），再根据对齐方式裁剪出指定高度的区域，
    /// 然后将每个像素二值化（RGB 任一通道低于 128 为黑，否则为白），
    /// 每 8 个像素编码为 1 字节，最终输出 Base64 字符串。
    /// 此格式用于 Macro Deck 客户端的单色显示屏。
    /// </summary>
    /// <param name="originalBitmap">原始位图</param>
    /// <param name="size">目标尺寸（Width 为正方形边长，Height 为裁剪高度）</param>
    /// <param name="contentAlignment">垂直裁剪对齐方式</param>
    /// <returns>二值化后的 Base64 编码字符串，失败返回空字符串</returns>
    public static string GetBase64ByteArray(Bitmap originalBitmap,
        Size size,
        ContentAlignment contentAlignment = ContentAlignment.MiddleCenter)
    {
        // 根据对齐方式计算裁剪区域的 Y 偏移
        var section = new Rectangle(new Point(0, size.Width / 2 - size.Height / 2), new Size(size.Width, size.Height));
        switch (contentAlignment)
        {
            case ContentAlignment.TopLeft:
            case ContentAlignment.TopCenter:
            case ContentAlignment.TopRight:
                section = new Rectangle(new Point(0, 0), new Size(size.Width, size.Height));
                break;
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.MiddleRight:
                section = new Rectangle(new Point(0, size.Width / 2 - size.Height / 2),
                    new Size(size.Width, size.Height));
                break;
            case ContentAlignment.BottomLeft:
            case ContentAlignment.BottomCenter:
            case ContentAlignment.BottomRight:
                section = new Rectangle(new Point(0, size.Width - size.Height), new Size(size.Width, size.Height));
                break;
        }

        try
        {
            // 先缩放为正方形，再裁剪出目标区域
            using var scaledBitmap = new Bitmap(originalBitmap, new Size(size.Width, size.Width));
            using var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(scaledBitmap, 0, 0, section, GraphicsUnit.Pixel);
            }

            var totalPixels = bitmap.Width * bitmap.Height;
            var currentPixels = 0;
            var colorByte = 0;
            var bitInBlock = 7;

            var result = new byte[totalPixels / 8];

            // 使用 LockBits 直接操作像素数据以提高性能
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var ptr = bitmapData.Scan0;
            var bytes = bitmapData.Stride * bitmap.Height;
            var rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            var pixelByte = 0;

            // 逐像素二值化：每 8 个像素编码为 1 字节
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    pixelByte = (y * bitmap.Width + x) * 4;

                    // 判断像素是否为黑色（RGB 任一通道低于 128）
                    if (rgbValues[pixelByte + 2] < 128 || // Red
                        rgbValues[pixelByte + 1] < 128 || // Green
                        rgbValues[pixelByte + 0] < 128) // Blue
                    {
                        colorByte &= ~(1 << bitInBlock); // 黑色/清除位
                    }
                    else
                    {
                        colorByte |= 1 << bitInBlock; // 白色/设置位
                    }

                    if (bitInBlock == 0)
                    {
                        result[currentPixels] = (byte)colorByte;
                        currentPixels++;
                        bitInBlock = 7;
                        colorByte = 0;
                    }
                    else
                    {
                        bitInBlock--;
                    }
                }
            }

            return Convert.ToBase64String(result);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 从 Base64 字符串解码为 Image 对象。
    /// 自动补全 Base64 填充字符（=），处理不规范的 Base64 输入。
    /// </summary>
    /// <param name="base64">Base64 编码的图像数据</param>
    /// <returns>解码后的 Image 对象，失败返回 null</returns>
    public static Image GetImageFromBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return null;
        }

        try
        {
            // 补全 Base64 填充字符
            var whiteSpace = new HashSet<char> { '\t', '\n', '\r', ' ' };
            var length = base64.Count(c => !whiteSpace.Contains(c));
            if (length % 4 != 0)
            {
                base64 += new string('=', 4 - length % 4);
            }

            var imageBytes = Convert.FromBase64String(base64);

            var ms = new MemoryStream(imageBytes);
            Image image = Image.FromStream(ms, true);

            return image;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 将 Image 对象编码为 Base64 字符串。
    /// GIF 格式直接保存原始格式，其他格式转换为 PNG。
    /// 非格式图像会创建临时位图以避免 GDI+ 错误。
    /// 注意：不会释放传入的 Image 对象，调用者保留所有权。
    /// </summary>
    /// <param name="image">要编码的图像</param>
    /// <returns>Base64 编码字符串，失败返回空字符串</returns>
    public static string GetBase64FromImage(Image image)
    {
        if (image == null)
        {
            return "";
        }

        Bitmap tempBitmap = null;
        try
        {
            using var ms = new MemoryStream();
            var format = image.RawFormat;
            var imageToSave = image;
            switch (format.ToString().ToLower())
            {
                case "gif":
                    // GIF 格式直接保存，保留动画帧
                    break;
                default:
                    // 非 GIF 格式创建新位图以避免 GDI+ 错误
                    tempBitmap = new Bitmap(image);
                    imageToSave = tempBitmap;
                    format = ImageFormat.Png;
                    break;
            }

            imageToSave.Save(ms, format);

            return Convert.ToBase64String(ms.ToArray());
        }
        catch
        {
            return "";
        }
        finally
        {
            tempBitmap?.Dispose();
        }
    }
}
