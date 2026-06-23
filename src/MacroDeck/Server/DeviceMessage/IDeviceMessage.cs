namespace SuchByte.MacroDeck.Server.DeviceMessage;

/// <summary>
/// 设备消息接口，定义了向客户端设备发送消息的标准方法。
/// 不同类型的设备可以实现此接口以提供特定的消息格式。
/// </summary>
public interface IDeviceMessage
{
    /// <summary>
    /// 客户端连接成功后发送初始配置和按钮数据
    /// </summary>
    /// <param name="macroDeckClient">已连接的客户端</param>
    public void Connected(MacroDeckClient macroDeckClient);

    /// <summary>
    /// 发送配置信息（行列数、间距、圆角、亮度等）
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    public void SendConfiguration(MacroDeckClient macroDeckClient);

    /// <summary>
    /// 发送当前文件夹的所有按钮数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    public void SendAllButtons(MacroDeckClient macroDeckClient);

    /// <summary>
    /// 更新单个按钮的数据
    /// </summary>
    /// <param name="macroDeckClient">目标客户端</param>
    /// <param name="actionButton">要更新的按钮</param>
    public void UpdateButton(MacroDeckClient macroDeckClient, ActionButton.ActionButton actionButton);
}
