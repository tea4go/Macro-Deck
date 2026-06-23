namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// Double 类型扩展方法类，提供字节到兆字节的转换
/// </summary>
public static class DoubleExtensions
{
    /// <summary>
    /// 将字节数转换为兆字节数（MB）
    /// </summary>
    /// <param name="bytes">字节数</param>
    /// <returns>兆字节数</returns>
    public static double ConvertBytesToMegabytes(this double bytes)
    {
        return bytes / 1024.0f / 1024.0f;
    }
}
