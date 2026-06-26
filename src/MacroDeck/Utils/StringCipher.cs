using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 字符串加密/解密工具类，使用 AES（Rijndael）对称加密算法。
/// 加密时随机生成 Salt 和 IV，并拼接到密文前面，解密时从中提取。
/// 使用 PBKDF2（Rfc2898DeriveBytes）从密码派生密钥。
/// 还提供获取 Windows 机器 GUID 的方法，用于生成机器绑定的加密密钥。
/// </summary>
public static class StringCipher
{
    /// <summary>加密算法密钥大小（位）</summary>
    private const int Keysize = 128;

    /// <summary>PBKDF2 密钥派生迭代次数</summary>
    private const int DerivationIterations = 1000;

    /// <summary>
    /// 使用密码短语加密明文字符串。
    /// 每次加密随机生成 Salt 和 IV，拼接格式：[Salt 16字节] + [IV 16字节] + [密文]。
    /// </summary>
    /// <param name="plainText">要加密的明文</param>
    /// <param name="passPhrase">加密密码短语</param>
    /// <returns>Base64 编码的密文字符串</returns>
    public static string Encrypt(string plainText, string passPhrase)
    {
        // 每次加密随机生成 Salt 和 IV，拼接在密文前面以便解密时提取
        var saltStringBytes = Generate128BitsOfRandomEntropy();
        var ivStringBytes = Generate128BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = Rfc2898DeriveBytes.Pbkdf2(
            passPhrase, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA1, Keysize / 8);
        using var symmetricKey = Aes.Create();
        symmetricKey.BlockSize = 128;
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.PKCS7;
        using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        // 拼接最终密文：Salt + IV + 加密数据
        var cipherTextBytes = saltStringBytes;
        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }

    /// <summary>
    /// 使用密码短语解密密文字符串。
    /// 从密文中提取 Salt、IV 和加密数据，然后使用相同的算法解密。
    /// </summary>
    /// <param name="cipherText">Base64 编码的密文</param>
    /// <param name="passPhrase">解密密码短语</param>
    /// <returns>解密后的明文字符串</returns>
    public static string Decrypt(string cipherText, string passPhrase)
    {
        // 密文格式：[Salt 16字节] + [IV 16字节] + [加密数据]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        // 提取前 16 字节作为 Salt
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        // 提取接下来的 16 字节作为 IV
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // 剩余部分为实际加密数据
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8 * 2)
            .Take(cipherTextBytesWithSaltAndIv.Length - Keysize / 8 * 2).ToArray();

        var keyBytes = Rfc2898DeriveBytes.Pbkdf2(
            passPhrase, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA1, Keysize / 8);
        using var symmetricKey = Aes.Create();
        symmetricKey.BlockSize = 128;
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.PKCS7;
        using var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
        using var memoryStream = new MemoryStream(cipherTextBytes);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream, Encoding.UTF8);
        return streamReader.ReadToEnd();
    }

    /// <summary>
    /// 生成 128 位（16 字节）的密码学安全随机数，用作 Salt 或 IV
    /// </summary>
    /// <returns>16 字节的随机字节数组</returns>
    private static byte[] Generate128BitsOfRandomEntropy()
    {
        var randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);
        return randomBytes;
    }

    /// <summary>
    /// 获取当前 Windows 机器的 GUID。
    /// 从注册表 SOFTWARE\Microsoft\Cryptography 下的 MachineGuid 值读取。
    /// 该 GUID 在系统安装时生成，可用于生成机器绑定的加密密钥。
    /// </summary>
    /// <returns>机器 GUID 字符串</returns>
    /// <exception cref="KeyNotFoundException">注册表键不存在时抛出</exception>
    /// <exception cref="IndexOutOfRangeException">注册表值不存在时抛出</exception>
    public static string GetMachineGuid()
    {
        var location = @"SOFTWARE\Microsoft\Cryptography";
        var name = "MachineGuid";

        using var localMachineX64View =
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using var rk = localMachineX64View.OpenSubKey(location);
        if (rk == null)
        {
            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
        }

        var machineGuid = rk.GetValue(name);
        if (machineGuid == null)
        {
            throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
        }

        return machineGuid.ToString();
    }
}
