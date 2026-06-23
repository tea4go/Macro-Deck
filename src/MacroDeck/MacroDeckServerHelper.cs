using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck;

/// <summary>
/// Macro Deck 服务器辅助类，负责配置和启动 ASP.NET Core Web 主机。
/// 支持 HTTP 和 HTTPS（需要提供 SSL 证书）两种模式。
/// </summary>
public static class MacroDeckServerHelper
{
    private static IHost? _host;

    /// <summary>
    /// 是否使用 HTTPS 协议
    /// </summary>
    internal static bool UseHttps { get; private set; }

    /// <summary>
    /// 配置并启动 Web 服务器主机。
    /// 如果已有运行中的主机实例，先停止再重新启动。
    /// 当提供了 SSL 证书时启用 HTTPS，否则仅使用 HTTP。
    /// </summary>
    /// <param name="port">服务器监听端口号</param>
    /// <param name="certificate">SSL 证书，为 null 时使用 HTTP</param>
    public static async Task Setup(int port, X509Certificate2? certificate)
    {
        // 如果已有运行中的主机，先停止
        if (_host is not null)
        {
            await _host.StopAsync();
        }

        // 根据是否提供证书决定是否启用 HTTPS
        UseHttps = certificate is not null;

        _host = Host.CreateDefaultBuilder()
            .ConfigureSerilog()
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.UseStartup<ServerStartup>();
                hostBuilder.ConfigureKestrel(options =>
                {
                    if (UseHttps)
                    {
                        // HTTPS 模式：配置 SSL 证书，并限制仅使用 HTTP/1 协议
                        options.ListenAnyIP(port,
                            listenOptions =>
                            {
                                listenOptions.UseHttps(certificate!);
                                listenOptions.Protocols = HttpProtocols.Http1;
                            });
                    }
                    else
                    {
                        // HTTP 模式：直接监听所有网络接口
                        options.ListenAnyIP(port);
                    }
                });
            }).Build();

        await _host.RunAsync();
    }
}
