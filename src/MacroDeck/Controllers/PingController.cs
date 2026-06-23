using Microsoft.AspNetCore.Mvc;
using SuchByte.MacroDeck.DataTypes;

namespace SuchByte.MacroDeck.Controllers;

/// <summary>
/// Ping 控制器，提供简单的健康检查端点。
/// 客户端可通过此端点检测 Macro Deck 服务器是否在线。
/// </summary>
[Route("ping")]
public class PingController : ControllerBase
{
    /// <summary>
    /// 处理 Ping 请求，返回包含机器名的响应
    /// </summary>
    /// <returns>包含机器名的 Ping 响应</returns>
    [HttpGet]
    public ActionResult<PingResponse> Ping()
    {
        return new PingResponse();
    }
}
