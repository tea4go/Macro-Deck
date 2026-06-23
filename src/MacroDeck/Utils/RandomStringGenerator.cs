using System.Security.Cryptography;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 随机字符串生成器，使用密码学安全的随机数生成器
/// </summary>
public class RandomStringGenerator
{
    /// <summary>
    /// 生成指定长度的随机字符串，包含大小写字母和数字。
    /// 使用 RandomNumberGenerator 确保密码学安全性。
    /// </summary>
    /// <param name="length">字符串长度</param>
    /// <returns>随机字符串</returns>
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomNumberGenerator.GetInt32(s.Length - 1)]).ToArray());
    }
}
