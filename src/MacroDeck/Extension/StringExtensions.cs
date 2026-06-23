using System.Security.Cryptography;

namespace SuchByte.MacroDeck.Extension;

/// <summary>
/// String 扩展方法类，提供密码学安全的字符串比较方法。
/// 使用 SHA-256 哈希后逐字节比较，防止时序攻击。
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 密码学安全的字符串相等比较。
    /// 先对两个字符串分别计算 SHA-256 哈希，然后逐字节比较哈希值，
    /// 避免因比较耗时差异导致时序攻击。
    /// </summary>
    /// <param name="str1">第一个字符串</param>
    /// <param name="str2">第二个字符串</param>
    /// <returns>两个字符串相等返回 true，否则返回 false</returns>
    public static bool EqualsCryptographically(this string? str1, string? str2)
    {
        if (str1 == null || str2 == null)
        {
            return false;
        }

        var hash1 = SHA256.HashData(Encoding.UTF8.GetBytes(str1));
        var hash2 = SHA256.HashData(Encoding.UTF8.GetBytes(str2));

        if (hash1.Length != hash2.Length)
        {
            return false;
        }

        // 逐字节比较，确保比较时间不依赖于差异位置
        return !hash1.Where((t, i) => t != hash2[i]).Any();
    }
}
