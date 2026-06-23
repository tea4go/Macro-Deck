using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck;

/// <summary>
/// ASP.NET Core 服务器启动配置类，负责配置依赖注入服务和 HTTP 请求管道。
/// 配置 CORS、HTTPS 重定向、静态文件服务、WebSocket 支持和路由映射。
/// </summary>
public class ServerStartup
{
    /// <summary>
    /// 配置依赖注入服务，注册 REST API 控制器
    /// </summary>
    /// <param name="services">服务集合</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterRestApiControllers();
    }

    /// <summary>
    /// 配置 HTTP 请求处理管道。
    /// 顺序：CORS → HTTPS 重定向（如启用）→ 静态文件服务 → WebSocket → 路由 → 端点映射
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <param name="env">Web 主机环境</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 允许任意来源的跨域请求
        app.UseCors("AllowAny");

        // 启用 HTTPS 时强制将 HTTP 请求重定向到 HTTPS
        if (MacroDeckServerHelper.UseHttps)
        {
            app.UseHttpsRedirection();
        }

        // 启用静态文件服务（用于提供 Web 客户端文件）
        app.UseFileServer();

        // 启用 WebSocket 支持，设置 2 分钟保活间隔
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        });

        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
