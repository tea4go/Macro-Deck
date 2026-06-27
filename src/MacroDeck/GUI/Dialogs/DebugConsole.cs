using System.Diagnostics;
using System.IO;
using Serilog;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Notifications;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;
using Form = SuchByte.MacroDeck.GUI.CustomControls.Form;

namespace SuchByte.MacroDeck.GUI.Dialogs;

/// <summary>
/// 调试控制台窗口，用于实时查看应用程序日志输出、筛选日志来源、切换日志级别、
/// 导出日志内容以及执行快速操作（重启、退出、打开目录等）。
/// 该窗口运行在独立的 UI 线程上，确保即使主应用繁忙也能保持响应。
/// </summary>
public partial class DebugConsole : Form
{
    /// <summary>
    /// 调试控制台专用的日志记录器实例
    /// </summary>
    private static readonly ILogger Logger = Log.ForContext(typeof(DebugConsole));

    /// <summary>
    /// 当前打开的调试控制台实例，为 null 表示未打开。
    /// 由 <see cref="Logging.DebugConsoleSink"/> 用于将日志事件转发到窗口中显示。
    /// </summary>
    public static DebugConsole? Current { get; private set; }

    /// <summary>
    /// 窗口加载时的原始客户区尺寸，用于后续字体自适应布局计算
    /// </summary>
    private Size _originalClientSize;

    /// <summary>日志输出区初始字体大小（磅），用于 Ctrl+= 恢复默认字号</summary>
    private float _originalLogFontSize;

    /// <summary>
    /// 在独立的 UI 线程上启动调试控制台窗口。
    /// 独立线程确保控制台不受主应用程序启动过程的影响，始终保持响应。
    /// </summary>
    public static void Launch()
    {
        // 创建后台线程运行调试控制台的消息循环
        var thread = new Thread(() =>
        {
            Current = new DebugConsole();
            Application.Run(Current);
        })
        {
            Name = "DebugConsole",
            IsBackground = true
        };
        // WinForms 控件必须运行在 STA 线程模型下
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    /// <summary>
    /// 构造函数：初始化控件、应用本地化文本、恢复上次的筛选器设置，并初始化日志级别下拉列表。
    /// </summary>
    public DebugConsole()
    {
        InitializeComponent();

        // 设置日志输出区使用等宽中文字体，便于对齐阅读
        logOutput.Font = new Font("Microsoft YaHei Mono", logOutput.Font.Size, FontStyle.Regular);

        ApplyLanguage();
        if (!string.IsNullOrWhiteSpace(Settings.Default.DebugConsoleFilters))
        {
            filter.Text = Settings.Default.DebugConsoleFilters;
        }

        foreach (var logLevels in (LogLevel[])Enum.GetValues(typeof(LogLevel)))
        {
            logLevel.Items.Add(logLevels.ToString());
        }

        logLevel.Text = MacroDeckLogger.LogLevel.ToString();
        FormClosed += DebugConsole_FormClosed;
        filtersList.ItemClicked += FiltersList_ItemClicked;
        filter.TextChanged += Filter_TextChanged;

        // 启用键预览以捕获 Ctrl+加号/减号/等号 调整日志区字体大小
        KeyPreview = true;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_originalClientSize.IsEmpty) _originalClientSize = ClientSize;
        LayoutHelper.AdjustAllLabelHeights(this);
        LayoutHelper.AdjustFormToFitControls(this, _originalClientSize);
        // 重放自调试控制台打开以来缓存的所有日志事件
        DebugConsoleSink.Replay();

        // 记录日志输出区的初始字体大小，作为 Ctrl+= 恢复的目标值
        _originalLogFontSize = logOutput.Font.Size;
    }

    /// <summary>
    /// 处理键盘快捷键：Ctrl+加号放大日志字体，Ctrl+减号缩小，Ctrl+= 恢复初始大小。
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Ctrl+= → 加号键，放大日志字体
        if (e.Control && e.KeyCode == Keys.Oemplus)
        {
            AdjustLogFontSize(2);
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        // Ctrl+- → 减号键，缩小日志字体
        if (e.Control && e.KeyCode == Keys.OemMinus)
        {
            AdjustLogFontSize(-2);
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        // Ctrl+0 → 数字零键，恢复初始字体大小
        if (e.Control && e.KeyCode == Keys.D0)
        {
            logOutput.Font = new Font(logOutput.Font.FontFamily, _originalLogFontSize, logOutput.Font.Style);
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        base.OnKeyDown(e);
    }

    /// <summary>
    /// 按指定增量调整日志输出区的字体大小，限制在 6～32 磅范围内。
    /// </summary>
    /// <param name="delta">字体磅数增量，正数放大，负数缩小</param>
    private void AdjustLogFontSize(float delta)
    {
        float newSize = logOutput.Font.Size + delta;
        if (newSize < 6f) newSize = 6f;
        if (newSize > 32f) newSize = 32f;
        logOutput.Font = new Font(logOutput.Font.FontFamily, newSize, logOutput.Font.Style);
    }

    /// <summary>
    /// 将窗口上所有静态文本控件（按钮、标签）设置为当前语言环境的本地化文本。
    /// </summary>
    private void ApplyLanguage()
    {
        btnClear.Text = LanguageManager.Strings.DebugConsoleClear;
        btnRestartMacroDeck.Text = LanguageManager.Strings.Restart;
        btnExit.Text = LanguageManager.Strings.Exit;
        btnOpenUser.Text = LanguageManager.Strings.DebugConsoleOpenUserDirectory;
        label1.Text = LanguageManager.Strings.DebugConsoleLogLevel;
        btnExportOutput.Text = LanguageManager.Strings.DebugConsoleExportOutput;
        label3.Text = LanguageManager.Strings.DebugConsoleFilter;
        btnTestNotification.Text = LanguageManager.Strings.DebugConsoleTestNotification;
        btnOpenLogs.Text = LanguageManager.Strings.DebugConsoleOpenLogs;
    }

    /// <summary>
    /// 筛选器文本框内容变更时，将新的筛选条件持久化保存到用户设置中。
    /// </summary>
    private void Filter_TextChanged(object sender, EventArgs e)
    {
        Settings.Default.DebugConsoleFilters = filter.Text;
        Settings.Default.Save();
    }

    /// <summary>
    /// 调试控制台窗口关闭时，仅关闭查看器而 Macro Deck 继续运行。
    /// 如需退出程序请使用"退出 Macro Deck"按钮或托盘图标。
    /// 独立的 UI 线程在消息循环结束后会干净地终止。
    /// </summary>
    private void DebugConsole_FormClosed(object sender, FormClosedEventArgs e)
    {
        Current = null;
    }

    /// <summary>
    /// 向日志输出区域追加文本，支持按发送者筛选和自定义颜色。
    /// 该方法会将操作投递到控制台自身的 UI 线程上执行，确保线程安全。
    /// </summary>
    /// <param name="text">要追加的日志文本内容</param>
    /// <param name="sender">日志发送者名称（如 "Macro Deck" 或插件名），用于筛选匹配</param>
    /// <param name="color">文本显示颜色</param>
    public void AppendText(string text, string sender, Color color)
    {
        // 控件已释放或句柄未创建时直接丢弃消息
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            // 将追加操作以异步方式投递到控制台 UI 线程的消息队列中执行
            BeginInvoke(() =>
            {
                try
                {
                    // 二次检查：执行时控件可能已被释放
                    if (IsDisposed || logOutput.IsDisposed)
                    {
                        return;
                    }

                    // 根据筛选器文本过滤日志来源：筛选器为空则不过滤，否则仅显示匹配来源的日志
                    if (!string.IsNullOrWhiteSpace(filter.Text))
                    {
                        var filters = filter.Text.Split(";")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();
                        if (filters.Length > 0 && Array.IndexOf(filters, sender) == -1)
                        {
                            return;
                        }
                    }

                    // 将光标定位到文本末尾，设置颜色后追加文本，然后滚动到最新位置
                    logOutput.SelectionStart = logOutput.TextLength;
                    logOutput.SelectionLength = 0;

                    logOutput.SelectionColor = color;
                    logOutput.AppendText(text);
                    logOutput.SelectionColor = logOutput.ForeColor;
                    logOutput.ScrollToCaret();
                }
                catch
                {
                    // 窗口正在释放/关闭——丢弃该日志消息
                }
            });
        }
        catch (Exception)
        {
            // 窗口正在释放/关闭——丢弃该日志消息
        }
    }

    /// <summary>
    /// 清空日志输出区域的全部内容。
    /// </summary>
    private void BtnClear_Click(object sender, EventArgs e)
    {
        logOutput.Text = string.Empty;
    }

    /// <summary>
    /// 使用当前启动参数重新启动 Macro Deck 应用程序。
    /// </summary>
    private void BtnRestartMacroDeck_Click(object sender, EventArgs e)
    {
        MacroDeck.RestartMacroDeck(string.Join(" ", MacroDeck.StartParameters));
    }

    /// <summary>
    /// 立即退出整个 Macro Deck 进程。
    /// </summary>
    private void BtnExit_Click(object sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    /// <summary>
    /// 使用系统默认文件管理器打开 Macro Deck 的用户数据目录。
    /// </summary>
    private void BtnOpenUser_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo(ApplicationPaths.UserDirectoryPath)
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// 找到最新的日志文件并使用 tail4go -f 命令实时跟踪日志输出。
    /// 如果无法找到日志文件则回退到打开日志目录。
    /// </summary>
    private void BtnOpenLogs_Click(object sender, EventArgs e)
    {
        // 尝试定位最近修改的日志文件
        string? logFilePath = null;
        try
        {
            var newest = new DirectoryInfo(ApplicationPaths.LogsDirectoryPath).GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();
            logFilePath = newest?.FullName;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "无法确定最新的日志文件");
        }

        // 如果找到了日志文件，使用 tail4go -f 实时跟踪；否则回退到打开日志目录
        if (logFilePath != null)
        {
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("tail4go")
                    {
                        Arguments = $"-f \"{logFilePath}\"",
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "无法启动 tail4go，请确保 tail4go 已安装并位于 PATH 中");
            }
        }
        else
        {
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(ApplicationPaths.LogsDirectoryPath)
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "无法打开日志目录");
            }
        }
    }

    /// <summary>
    /// 根据下拉列表选择切换全局日志级别，低于该级别的日志将不会输出。
    /// </summary>
    private void LogLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Enum.TryParse(typeof(LogLevel), this.logLevel.SelectedItem.ToString(), true, out var logLevel))
        {
            MacroDeckLogger.LogLevel = (LogLevel)logLevel;
        }
    }

    /// <summary>
    /// 将当前日志输出区域的纯文本内容导出为 .log 文件。
    /// 默认文件名为包含时间戳的 "debug_output_{时间}.log"。
    /// </summary>
    private void BtnExportOutput_Click(object sender, EventArgs e)
    {
        using var saveFileDialog = new SaveFileDialog
        {
            AddExtension = true,
            Filter = ".log|*.log",
            FileName = string.Format("debug_output_{0}.log", DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"))
        };
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                logOutput.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                Logger.Information("调试控制台输出已成功导出到：" + saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "导出调试控制台输出时发生错误");
            }
        }
    }

    /// <summary>
    /// 弹出筛选器下拉菜单，列出 "Macro Deck" 和所有已加载插件的名称，
    /// 点击某一项即可将其添加到筛选条件中。
    /// </summary>
    private void BtnAddFilter_Click(object sender, EventArgs e)
    {
        filtersList.Items.Clear();
        filtersList.Items.Add("Macro Deck").ForeColor = Color.White;
        foreach (var plugin in PluginManager.Plugins.Values)
        {
            filtersList.Items.Add(plugin.Name).ForeColor = Color.White;
        }

        // 在鼠标当前位置弹出筛选列表
        filtersList.Show(Cursor.Position);
    }

    /// <summary>
    /// 筛选器下拉菜单项点击处理：将选中的来源名称追加到筛选文本框中。
    /// </summary>
    private void FiltersList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        filter.Text += ";" + e.ClickedItem.Text;
    }

    /// <summary>
    /// 窗口加载事件处理（占位，具体初始化逻辑见 <see cref="OnLoad"/>）。
    /// </summary>
    private void DebugConsole_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// 清除所有筛选条件，恢复显示全部日志来源。
    /// </summary>
    private void btnRemoveFilters_Click(object sender, EventArgs e)
    {
        filter.Text = string.Empty;
    }

    /// <summary>
    /// 发送每个日志级别各一条测试日志，用于验证日志颜色和格式配置；
    /// 同时发送一条系统通知验证通知功能。
    /// </summary>
    private void btnTestNotification_Click(object sender, EventArgs e)
    {
        Logger.Verbose("[测试] ============================================================");

        Logger.Verbose("[测试] Verbose 级别日志 —— 最详细的追踪信息");
        Logger.Debug("[测试] Debug 级别日志 —— 调试诊断信息");
        Logger.Information("[测试] Information 级别日志 —— 常规运行信息");
        Logger.Warning("[测试] Warning 级别日志 —— 警告需要注意");
        Logger.Error("[测试] Error 级别日志 —— 发生了一般错误");
        Logger.Fatal("[测试] Fatal 级别日志 —— 致命错误需要立即处理");
        Logger.Verbose("[测试] ============================================================");

        NotificationManager.SystemNotification("测试", $"测试通知，发送时间：{DateTime.Now}", true);
    }
}
