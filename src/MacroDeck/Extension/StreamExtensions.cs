using System.IO;
using System.Security.Cryptography;

namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// Stream 扩展方法类，提供流数据的 SHA-256 哈希计算功能。
/// 使用增量哈希计算方式，避免将整个流加载到内存中。
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// 异步计算流的 SHA-256 哈希值。
    /// 使用 8KB 缓冲区分块读取和计算，适用于大文件。
    /// 计算完成后将流位置重置到起始位置。
    /// </summary>
    /// <param name="stream">要计算哈希的流</param>
    /// <returns>小写的十六进制格式哈希字符串</returns>
    /// <exception cref="InvalidOperationException">当哈希计算结果为 null 时抛出</exception>
    public static async ValueTask<string> CalculateSha256Hash(this Stream stream)
    {
        stream.Position = 0;

        var bufferedStream = new BufferedStream(stream);
        using var sha256 = SHA256.Create();

        // 使用增量方式计算哈希，避免一次性加载整个流到内存
        var buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = await bufferedStream.ReadAsync(buffer)) > 0)
        {
            sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
        }

        sha256.TransformFinalBlock(buffer, 0, 0);

        if (sha256.Hash == null)
        {
            throw new InvalidOperationException("哈希值为空");
        }

        // 重置流位置以便后续读取
        stream.Position = 0;
        return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
    }
}
