using System.Text.Json.Serialization; 
using Microsoft.AspNetCore.Mvc.ApplicationParts; 
using Microsoft.Extensions.DependencyInjection; 
 
namespace SuchByte.MacroDeck.StartupConfig; 
 
/// <summary> 
/// ASP.NET Core API 控制器注册配置类。 
/// 负责注册 REST API 控制器、配置 CORS 策略以及 JSON 序列化选项。 
/// </summary> 
public static class ApiControllerConfig 
{ 
    /// <summary> 
    /// 向服务容器注册 REST API 控制器及相关配置。 
    /// 包括 CORS 策略（允许任何来源）、MVC 服务、控制器服务以及 JSON 序列化选项（枚举转字符串、允许尾随逗号）。 
    /// </summary> 
    /// <param name="services">ASP.NET Core 服务集合。</param> 
    public static void RegisterRestApiControllers(this IServiceCollection services) 
    { 
        var assembly = typeof(MacroDeckServerHelper).Assembly; 
        // 配置 CORS 策略：允许任何来源、任意请求头和请求方法，同时允许携带凭据 
        services.AddCors(options => 
        { 
            options.AddPolicy("AllowAny", 
                builder => 
                    builder.SetIsOriginAllowed(_ => true) 
                        .AllowAnyHeader() 
                        .AllowAnyMethod() 
                        .AllowCredentials() 
                        .Build()); 
        }); 
        // 添加 MVC 核心服务 
        services.AddMvc(); 
        // 添加控制器服务并配置 JSON 序列化选项 
        services.AddControllers() 
            .AddJsonOptions(opt => 
            { 
                // 将枚举值序列化为字符串而非数字 
                var enumConverter = new JsonStringEnumConverter(); 
                opt.JsonSerializerOptions.Converters.Add(enumConverter); 
                // 允许 JSON 反序列化时容忍尾随逗号 
                opt.JsonSerializerOptions.AllowTrailingCommas = true; 
            }) 
            // 注册包含 API 控制器的程序集 
            .PartManager.ApplicationParts.Add(new AssemblyPart(assembly)); 
    } 
} 
