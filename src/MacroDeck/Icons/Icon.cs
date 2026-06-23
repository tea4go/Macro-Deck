using System.Drawing.Drawing2D;
using System.IO;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Icons;

/// <summary>
/// 图标类，表示图标包中的一个图标。
/// 每次访问图标数据时从磁盘加载，不进行缓存，避免长期占用内存。
/// 调用者负责释放返回的 Image 对象。
/// </summary>
public class Icon
{
    private string _filePath;

    /// <summary>图标文件路径</summary>
    public string FilePath
    {
        get => _filePath;
        set => _filePath = value;
    }

    /// <summary>图标唯一标识</summary>
    public string IconId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 从磁盘加载图标图像。每次访问都从文件重新读取，不进行缓存。
    /// 返回的 Image 是新实例，调用者必须负责释放。
    /// </summary>
    public Image IconImage
    {
        get
        {
            using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;
            return Image.FromStream(ms);
        }
    }

    /// <summary>
    /// 加载图标并缩放到指定尺寸的缩略图。
    /// 仅保留缩略图位图，在网格中显示大量图标时保持低内存占用。
    /// 返回的 Image 是新实例，调用者必须负责释放。
    /// </summary>
    /// <param name="width">缩略图宽度</param>
    /// <param name="height">缩略图高度</param>
    /// <returns>缩放后的缩略图</returns>
    public Image GetThumbnail(int width, int height)
    {
        using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        using var source = Image.FromStream(fs);
        var thumbnail = new Bitmap(width, height);
        using var g = Graphics.FromImage(thumbnail);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(source, 0, 0, width, height);
        return thumbnail;
    }

    /// <summary>
    /// 按需计算图标的 Base64 编码字符串。不进行缓存，图标数据不会长期驻留内存。
    /// </summary>
    public string IconBase64
    {
        get
        {
            using var image = IconImage;
            return Base64.GetBase64FromImage(image);
        }
    }

    /// <summary>
    /// 获取图标缩放为 128x64 尺寸后的 Base64 编码字节数组。
    /// 用于客户端显示图标。
    /// </summary>
    public string IconHex128_64Base64
    {
        get
        {
            using var image = IconImage;
            return Base64.GetBase64ByteArray((Bitmap)image, new Size(128, 64));
        }
    }
}
