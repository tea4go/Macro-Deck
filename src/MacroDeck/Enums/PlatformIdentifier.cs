namespace SuchByte.MacroDeck.Enums;

/// <summary>
/// 平台标识符枚举，每个平台对应一个唯一数值 ID
/// </summary>
public enum PlatformIdentifier
{
    /// <summary>Windows x64</summary>
    WinX64 = 1000,
    /// <summary>macOS x64 (Intel)</summary>
    MacX64 = 2000,
    /// <summary>macOS ARM64 (Apple Silicon)</summary>
    MacArm64 = 2100,
    /// <summary>Linux x64</summary>
    LinuxX64 = 3000,
    /// <summary>Linux ARM64</summary>
    LinuxArm64 = 3100,
    /// <summary>Linux ARM32</summary>
    LinuxArm32 = 3110
}
