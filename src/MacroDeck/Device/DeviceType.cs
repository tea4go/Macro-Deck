namespace SuchByte.MacroDeck.Device;

/// <summary>
/// 设备类型枚举，区分不同平台的客户端设备
/// </summary>
public enum DeviceType
{
    /// <summary>未知设备类型</summary>
    Unknown,
    /// <summary>Web 浏览器客户端</summary>
    Web,
    /// <summary>Android 客户端</summary>
    Android,
    /// <summary>iOS 客户端</summary>
    iOS,
    /// <summary>Stream Deck 硬件设备</summary>
    StreamDeck
}
