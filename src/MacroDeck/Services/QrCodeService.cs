using System.IO;
using System.Text.Json;
using QRCoder;
using SuchByte.MacroDeck.DataTypes.QrCode;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Services;

/// <summary>
/// 二维码服务，用于生成快速设置二维码。
/// 客户端可通过扫描此二维码自动获取连接所需的信息（IP、端口、SSL 配置等）。
/// </summary>
public class QrCodeService
{
    /// <summary>
    /// QrCodeService 的单例实例。
    /// </summary>
    public static readonly QrCodeService Instance = new();

    /// <summary>
    /// 生成快速设置二维码图片。
    /// 将本机连接信息（机器名、网络接口、端口、SSL 配置、快速设置令牌）序列化为 JSON，
    /// 编码为 Base64 后嵌入到 URL 中，再生成相应的二维码图片。
    /// </summary>
    /// <returns>包含快速连接信息的二维码 Bitmap 图片。</returns>
    public Image GetQuickSetupQrCode()
    {
        // 获取本机所有网络接口信息
        var networkInterfaces = NetworkUtils.GetNetworkInterfaces();

        // 构建快速连接所需的数据对象
        var data = new QuickConnectQrCodeData(Environment.MachineName,
            networkInterfaces,
            MacroDeck.Configuration.HostPort,
            MacroDeck.Configuration.EnableSsl,
            MacroDeckServer.QuickSetupToken);

        // 将数据序列化为 JSON，使用 camelCase 命名策略
        var dataJson = JsonSerializer.Serialize(data,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        // JSON 转为 Base64 编码，嵌入到快速设置 URL 中
        var dataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataJson));
        var qrCodeLink = $"https://macro-deck.app/quick-setup/{dataBase64}";

        // 使用 QRCoder 生成二维码图片
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrCodeLink, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new PngByteQRCode(qrCodeData);

        var quickSetupQrCode = qrCode.GetGraphic(20, Color.White, Color.Transparent, false);
        using var ms = new MemoryStream(quickSetupQrCode);
        using var image = Image.FromStream(ms);
        // Return a copy that does not depend on the (disposed) stream
        return new Bitmap(image);
    }
}
