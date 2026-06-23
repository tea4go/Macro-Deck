namespace SuchByte.MacroDeck.Device;

/// <summary>
/// 设备配置类，包含设备显示和连接相关的配置选项
/// </summary>
public class DeviceConfiguration
{
    /// <summary>屏幕亮度，范围 0.0 到 1.0</summary>
    public float Brightness = 0.3f;
    /// <summary>是否自动连接到服务器</summary>
    public bool AutoConnect = false;
    /// <summary>屏幕唤醒锁定方式</summary>
    public WakeLockMethod WakeLockMethod = WakeLockMethod.Connected;
}

/// <summary>
/// 屏幕唤醒锁定方式枚举
/// </summary>
public enum WakeLockMethod
{
    /// <summary>始终保持唤醒</summary>
    Always,
    /// <summary>连接时保持唤醒</summary>
    Connected,
    /// <summary>从不保持唤醒</summary>
    Never
}
