namespace SuchByte.MacroDeck.DataTypes;

/// <summary>
/// Ping 响应数据类，用于 WebSocket 心跳检测的响应。
/// 包含当前机器名称，客户端可通过此响应确认服务端可达性。
/// </summary>
public class PingResponse
{
    /// <summary>当前机器名称</summary>
    public string MachineName { get; set; }

    /// <summary>
    /// 构造函数，自动填充当前机器名称
    /// </summary>
    public PingResponse()
    {
        MachineName = Environment.MachineName;
    }
}
