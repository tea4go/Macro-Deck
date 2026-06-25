using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.StartupConfig;

/// <summary>
/// 系统托盘图标设置类。
/// 负责配置系统托盘的 NotifyIcon，包括左键单击显示窗口以及右键菜单（显示、重启、退出）。
/// </summary>
public static class TrayIconSetup 
{ 
    /// <summary> 
    /// 配置系统托盘图标及其右键菜单。 
    /// 设置左键单击显示主窗口，右键菜单包含"显示"、"重启"和"退出"选项。 
    /// </summary> 
    /// <param name="trayIcon">要配置的 NotifyIcon 实例。</param> 
    /// <param name="trayIconContextMenu">托盘图标的右键菜单。</param> 
    /// <param name="showAction">显示主窗口的回调操作。</param> 
    /// <param name="restartAction">重启应用程序的回调操作。</param> 
    /// <param name="exitAction">退出应用程序的回调操作。</param> 
    /// <returns>配置完成的 NotifyIcon 实例。</returns> 
    public static NotifyIcon SetupTrayIcon(this NotifyIcon trayIcon, 
        ContextMenuStrip trayIconContextMenu, 
        Action showAction, 
        Action restartAction, 
        Action exitAction) 
    { 
        trayIcon.Visible = true; 
        // 注册鼠标按下事件：左键单击显示主窗口 
        trayIcon.MouseDown += (obj, e) => 
        { 
            if (e.Button == MouseButtons.Left) 
            { 
                showAction?.Invoke(); 
            } 
        }; 
 
        // 创建"显示"菜单项 
        var showItem = new ToolStripMenuItem 
        { 
            Text = LanguageManager.Strings.Show, 
            Font = new Font(FontManager.FontFamily, FontManager.FontSize, 
                FontManager.FontBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point) 
        }; 
        showItem.Click += (obj, e) => { showAction?.Invoke(); }; 
 
        // 创建"重启"菜单项 
        var restartItem = new ToolStripMenuItem 
        { 
            Text = LanguageManager.Strings.Restart, 
            Font = new Font(FontManager.FontFamily, FontManager.FontSize, 
                FontManager.FontBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point) 
        }; 
        restartItem.Click += (obj, e) => { restartAction?.Invoke(); }; 
 
        // 创建"退出"菜单项 
        var exitItem = new ToolStripMenuItem 
        { 
            Text = LanguageManager.Strings.Exit, 
            Font = new Font(FontManager.FontFamily, FontManager.FontSize, 
                FontManager.FontBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point) 
        }; 
        exitItem.Click += (obj, e) => { exitAction?.Invoke(); }; 
 
        // 将菜单项添加到右键菜单 
        trayIconContextMenu.Items.Add(showItem); 
        trayIconContextMenu.Items.Add(restartItem); 
        trayIconContextMenu.Items.Add(exitItem); 
        return trayIcon; 
    } 
} 
