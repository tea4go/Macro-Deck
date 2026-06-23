using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 自签名证书生成器，用于生成 Macro Deck 的 HTTPS 自签名证书。
/// 证书包含服务器身份验证扩展密钥用法、SAN（含 localhost 和本机所有 IPv4 地址），
/// 有效期 10 年，使用 RSA 2048 位密钥。
/// </summary>
public static class SelfSignedCertificateGenerator
{
    /// <summary>
    /// 生成自签名证书，返回 PEM 格式的证书和私钥。
    /// </summary>
    /// <returns>元组，包含证书 PEM 字符串和私钥 PEM 字符串</returns>
    public static (string CertPem, string KeyPem) Generate()
    {
        using var rsa = RSA.Create(2048);

        var req = new CertificateRequest("CN=Macro Deck Self-Signed",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        // TLS 服务器使用所需的密钥用法扩展
        req.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
            true));

        // 服务器身份验证扩展密钥用法
        req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
            new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") },
            false));

        // 基本约束扩展（非 CA）
        req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false,
            false,
            0,
            true));

        // 主题密钥标识符扩展
        req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

        // 构建 SAN（主题备用名称），包含 localhost 和本机所有 IPv4 地址
        var san = new SubjectAlternativeNameBuilder();
        san.AddDnsName("localhost");
        san.AddIpAddress(IPAddress.Loopback);
        san.AddIpAddress(IPAddress.IPv6Loopback);

        // 遍历所有网络接口，将活动的 IPv4 地址添加到 SAN
        foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (iface.OperationalStatus != OperationalStatus.Up)
            {
                continue;
            }

            foreach (var addr in iface.GetIPProperties().UnicastAddresses)
            {
                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    san.AddIpAddress(addr.Address);
                }
            }
        }

        req.CertificateExtensions.Add(san.Build());

        // 创建自签名证书，有效期 10 年
        var cert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
        var certPem = cert.ExportCertificatePem();
        var keyPem = rsa.ExportPkcs8PrivateKeyPem();

        return (certPem, keyPem);
    }
}
