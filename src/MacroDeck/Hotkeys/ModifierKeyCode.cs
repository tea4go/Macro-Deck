namespace SuchByte.MacroDeck.Hotkeys;

/// <summary>
/// 修饰键代码枚举，用于 Windows API 注册热键时的修饰键参数。
/// 这些值对应 Windows MOD_ALT、MOD_CONTROL、MOD_SHIFT 常量。
/// </summary>
public enum ModifierKeyCode
{
    /// <summary>Alt 键修饰</summary>
    ALT = 1,
    /// <summary>Ctrl 键修饰</summary>
    CTRL = 2,
    /// <summary>Shift 键修饰</summary>
    SHIFT = 4,
    /// <summary>Windows 键修饰</summary>
    WIN = 8
}
