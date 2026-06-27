using System.Runtime.InteropServices;
using Serilog;
using SuchByte.MacroDeck.Enums;
using SuchByte.MacroDeck.Server;

namespace SuchByte.MacroDeck.Hotkeys;

/// <summary>
/// 热键管理器，继承自 NativeWindow 以接收 Windows 热键消息。
/// 负责全局热键的注册、移除和触发处理。
/// 使用 Windows API（RegisterHotKey/UnregisterHotKey）实现系统级热键功能。
/// </summary>
public class HotkeyManager : NativeWindow
{
    private static readonly ILogger Logger = Log.ForContext(typeof(HotkeyManager));

    /// <summary>
    /// 注册热键的 Windows API 函数
    /// </summary>
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

    /// <summary>
    /// 取消注册热键的 Windows API 函数
    /// </summary>
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// 热键字典，键为动作按钮，值为热键 ID
    /// </summary>
    public static Dictionary<ActionButton.ActionButton, int> Hotkeys = new();

    private static Random random = new();

    /// <summary>
    /// 窗口句柄，用于接收热键消息
    /// </summary>
    private static IntPtr formHandle;

    /// <summary>
    /// 是否暂停热键处理
    /// </summary>
    private static bool paused;

    /// <summary>
    /// 构造函数，创建窗口句柄用于接收热键消息
    /// </summary>
    public HotkeyManager()
    {
        CreateHandle(new CreateParams());
        formHandle = Handle;
    }


    /// <summary>
    /// 为动作按钮注册全局热键。
    /// 先移除已有的热键注册，然后根据修饰键和键码注册新的热键。
    /// </summary>
    /// <param name="actionButton">关联的动作按钮</param>
    /// <param name="modifierKeys">修饰键（Ctrl、Shift、Alt 的组合）</param>
    /// <param name="key">热键键码</param>
    public static void AddHotkey(ActionButton.ActionButton actionButton, Keys modifierKeys, Keys key)
    {
        // 先移除已有的热键
        RemoveHotkey(actionButton);
        if (key == Keys.None)
        {
            return;
        }

        // 生成唯一的热键 ID
        var hotkeyId = random.Next(int.MaxValue);
        Hotkeys[actionButton] = hotkeyId;

        // 计算修饰键代码，支持组合修饰键
        var modifierKeyCode = 0;
        if ((modifierKeys & Keys.Control) == Keys.Control)
        {
            modifierKeyCode += (int)ModifierKeyCode.CTRL;
        }

        if ((modifierKeys & Keys.Shift) == Keys.Shift)
        {
            modifierKeyCode += (int)ModifierKeyCode.SHIFT;
        }

        if ((modifierKeys & Keys.Alt) == Keys.Alt)
        {
            modifierKeyCode += (int)ModifierKeyCode.ALT;
        }

        RegisterHotKey(formHandle, hotkeyId, modifierKeyCode, (int)key);
        Logger.Information("已注册热键 #{HotkeyId}（{Key}），修饰键：{Modifiers}",
            hotkeyId,
            key.ToString(),
            modifierKeys.ToString());
    }

    /// <summary>
    /// 暂停热键处理
    /// </summary>
    public static void Pause()
    {
        paused = true;
    }

    /// <summary>
    /// 恢复热键处理
    /// </summary>
    public static void Resume()
    {
        paused = false;
    }

    /// <summary>
    /// 移除动作按钮的热键注册
    /// </summary>
    /// <param name="actionButton">要移除热键的动作按钮</param>
    public static void RemoveHotkey(ActionButton.ActionButton actionButton)
    {
        if (!Hotkeys.ContainsKey(actionButton))
        {
            return;
        }

        var hotkeyId = Hotkeys[actionButton];
        UnregisterHotKey(formHandle, hotkeyId);
        Hotkeys.Remove(actionButton);
        Logger.Information("已注销热键 #{HotkeyId}", hotkeyId);
    }


    /// <summary>
    /// Windows 消息处理过程，拦截热键触发消息（0x0312 = WM_HOTKEY）。
    /// 当热键触发时，查找对应的动作按钮并执行短按动作。
    /// </summary>
    /// <param name="m">Windows 消息</param>
    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case 0x0312: // WM_HOTKEY 消息
                if (paused)
                {
                    break;
                }

                var hotkeyId = m.WParam.ToInt32();
                var actionButton = Hotkeys.FirstOrDefault(x => x.Value.Equals(hotkeyId)).Key;
                if (actionButton != null)
                {
                    try
                    {
                        // 热键触发时执行短按动作
                        MacroDeckServer.Execute(actionButton, "", ButtonPressType.SHORT);
                    }
                    catch
                    {
                    }
                }

                break;
        }

        base.WndProc(ref m);
    }
}
