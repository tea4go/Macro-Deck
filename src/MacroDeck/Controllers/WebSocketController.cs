using Microsoft.AspNetCore.Mvc;

namespace SuchByte.MacroDeck.Controllers;

/// <summary>
/// WebSocket 控制器，处理 WebSocket 连接请求。
/// 非 WebSocket 请求将被重定向到客户端页面，
/// WebSocket 请求将被接受并交由 WebSocketHandler 处理。
/// </summary>
public class WebSocketController : ControllerBase
{
    /// <summary>
    /// 处理根路径的 GET 请求。
    /// 如果是 WebSocket 请求则建立连接并处理，否则重定向到客户端页面。
    /// </summary>
    /// <returns>WebSocket 处理结果或重定向响应</returns>
    [Route("/")]
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return Redirect("client");
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await WebSocketHandler.HandleWebSocket(webSocket);
        return new EmptyResult();
    }
}
