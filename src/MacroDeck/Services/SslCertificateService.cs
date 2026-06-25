using System.Security.Cryptography.X509Certificates;
using Serilog;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Services;

/// <summary>
/// SSL 证书服务，负责管理和生成自签名 SSL 证书。
/// 支持创建 PEM 格式的证书和私钥，并进行加密存储与验证。
/// </summary>
public static class SslCertificateService
{
    /// <summary>
    /// SSL 证书服务的日志记录器。
    /// </summary>
    private static readonly ILogger Logger = Log.ForContext(typeof(SslCertificateService));

    /// <summary>
    /// 确保当前配置中存在有效的 SSL 证书。
    /// 如果启用了 SSL 但未配置证书，则自动生成自签名证书并保存。
    /// </summary>
    public static void EnsureValidCertificate()
    {
        // 如果未启用 SSL，无需处理证书
        if (!MacroDeck.Configuration.EnableSsl)
        {
            return;
        }

        // 如果证书和密钥均已配置，无需重新生成
        if (!string.IsNullOrWhiteSpace(MacroDeck.Configuration.SslCertificatePem) &&
            !string.IsNullOrWhiteSpace(MacroDeck.Configuration.SslCertificateKeyPemEncrypted))
        {
            return;
        }

        Logger.Information("No SSL certificate configured – generating self-signed certificate");
        // 生成并保存自签名证书
        var (certPem, keyPem) = SelfSignedCertificateGenerator.Generate();
        SaveCertificate(certPem, keyPem);
        Logger.Information("Self-signed certificate generated and saved");
    }

    /// <summary>
    /// 从配置中加载 X509 证书。
    /// 解密存储的私钥，结合证书 PEM 创建证书对象，再导出为 PKCS#12 格式加载。
    /// </summary>
    /// <returns>加载成功的 X509Certificate2 对象，如果证书/密钥为空或加载失败则返回 null。</returns>
    public static X509Certificate2? GetX509Certificate()
    {
        var certPem = MacroDeck.Configuration.SslCertificatePem;
        var keyEncrypted = MacroDeck.Configuration.SslCertificateKeyPemEncrypted;

        if (string.IsNullOrWhiteSpace(certPem) || string.IsNullOrWhiteSpace(keyEncrypted))
        {
            return null;
        }

        try
        {
            // 使用本机 GUID 解密存储的私钥
            var keyPem = StringCipher.Decrypt(keyEncrypted, StringCipher.GetMachineGuid());
            // 从 PEM 格式创建证书
            var cert = X509Certificate2.CreateFromPem(certPem, keyPem);
            // 导出为 PKCS#12 格式再重新加载，确保证书完整可用
            var pfxBytes = cert.Export(X509ContentType.Pfx);
            cert.Dispose();
            return X509CertificateLoader.LoadPkcs12(pfxBytes, null);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load SSL certificate");
            return null;
        }
    }

    /// <summary>
    /// 尝试验证给定的证书 PEM 和私钥 PEM 是否有效。
    /// 通过尝试从 PEM 创建证书对象来验证格式和内容的正确性。
    /// </summary>
    /// <param name="certPem">证书的 PEM 格式字符串。</param>
    /// <param name="keyPem">私钥的 PEM 格式字符串。</param>
    /// <param name="error">验证失败时输出错误信息，成功时为 null。</param>
    /// <returns>验证成功返回 true，失败返回 false。</returns>
    public static bool TryValidate(string certPem, string keyPem, out string? error)
    {
        if (string.IsNullOrWhiteSpace(certPem))
        {
            error = "Certificate PEM is empty.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(keyPem))
        {
            error = "Private key PEM is empty.";
            return false;
        }

        try
        {
            // 尝试从 PEM 创建证书对象以验证有效性
            using var cert = X509Certificate2.CreateFromPem(certPem, keyPem);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 保存证书和私钥到配置文件。
    /// 私钥使用本机 GUID 加密后存储，确保证书信息安全。
    /// </summary>
    /// <param name="certPem">证书的 PEM 格式字符串。</param>
    /// <param name="keyPem">私钥的 PEM 格式字符串（明文）。</param>
    public static void SaveCertificate(string certPem, string keyPem)
    {
        // 使用本机 GUID 对私钥进行加密
        var keyEncrypted = StringCipher.Encrypt(keyPem, StringCipher.GetMachineGuid());
        MacroDeck.Configuration.SslCertificatePem = certPem;
        MacroDeck.Configuration.SslCertificateKeyPemEncrypted = keyEncrypted;
        // 将配置持久化到文件
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }
}
