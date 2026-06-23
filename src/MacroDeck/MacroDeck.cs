using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SuchByte.MacroDeck.Backup;
using SuchByte.MacroDeck.Configuration;
using SuchByte.MacroDeck.DataTypes.Updater;
using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.GUI.MainWindowViews;
using SuchByte.MacroDeck.Hotkeys;
using Serilog;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Notifications;
using SuchByte.MacroDeck.Pipe;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Services;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables;
using Version = SuchByte.MacroDeck.DataTypes.Core.Version;

namespace SuchByte.MacroDeck;

/// <summary>
/// Macro Deck 主应用程序类，继承自 NativeWindow 以支持 Windows 消息处理。
/// 负责应用程序的启动、初始化、主窗口管理、系统托盘图标以及重启/退出等核心生命周期管理。
/// </summary>
public class MacroDeck : NativeWindow
{
    private static readonly ILogger Logger = Log.ForContext(typeof(MacroDeck));

    /// <summary>
    /// 当前 Macro Deck 版本号，从可执行文件的版本信息中获取
    /// </summary>
    public static Version Version =
        Version.Parse(FileVersionInfo.GetVersionInfo(ApplicationPaths.ExecutablePath).ProductVersion);

    /// <summary>
    /// 服务器 API 版本号
    /// </summary>
    public static readonly int ApiVersion = 20;

    /// <summary>
    /// 插件 API 版本号，插件需要兼容此版本才能正常加载
    /// </summary>
    public static readonly int PluginApiVersion = 20;

    /// <summary>
    /// 启动参数实例
    /// </summary>
    public static StartParameters StartParameters { get; private set; } = new();

    /// <summary>
    /// 主配置实例，包含所有用户可配置的设置项
    /// </summary>
    public static MainConfiguration Configuration { get; private set; } = new();

    /// <summary>
    /// 是否以安全模式运行
    /// </summary>
    public static bool SafeMode { get; set; } = false;

    /// <summary>
    /// UI 线程的同步上下文，用于跨线程调度到 UI 线程执行操作
    /// </summary>
    internal static SynchronizationContext? SyncContext { get; set; }

    /// <summary>
    /// 主窗口加载完成事件
    /// </summary>
    public static event EventHandler? OnMainWindowLoad;

    /// <summary>
    /// Macro Deck 完全加载完成事件
    /// </summary>
    public static event EventHandler? OnMacroDeckLoaded;

    private static readonly ContextMenuStrip TrayIconContextMenu = new();

    /// <summary>
    /// 系统托盘图标，用于最小化到托盘和显示通知
    /// </summary>
    private static readonly NotifyIcon TrayIcon = new()
    {
        Icon = Resources.appicon,
        Text = @$"Macro Deck {Version}",
        Visible = false,
        ContextMenuStrip = TrayIconContextMenu
    };

    private static MainWindow? _mainWindow;

    /// <summary>
    /// 获取主窗口实例，仅在窗口未被销毁、可见且句柄已创建时返回
    /// </summary>
    public static MainWindow? MainWindow =>
        _mainWindow is { IsDisposed: false, Visible: true, IsHandleCreated: true } ? _mainWindow : null;


    private static readonly Stopwatch StartUpTimeStopWatch = new();

    /// <summary>
    /// Macro Deck 应用程序的主入口点，负责初始化所有子系统并启动消息循环。
    /// 初始化顺序：日志 → 备份检查 → 临时文件清理 → 语言加载 → 配置加载 →
    /// 热键管理 → 变量管理 → 插件加载 → 图标管理 → 配置文件加载 →
    /// 网络搜索 → 服务器启动 → 广播服务 → ADB 服务 → 管道服务 → 托盘图标 → 主窗口
    /// </summary>
    /// <param name="startParameters">启动参数，包含端口、日志级别、调试控制台等选项</param>
    internal static void Start(StartParameters startParameters)
    {
        StartParameters = startParameters;
        StartUpTimeStopWatch.Start();

        // 根据启动参数设置日志级别
        if (StartParameters.LogLevel > 0)
        {
            MacroDeckLogger.LogLevel = (LogLevel)StartParameters.LogLevel;
        }

        // 如果启用了调试控制台参数，则启动调试控制台窗口
        if (StartParameters.DebugConsole)
        {
            DebugConsole.Launch();
        }

        Logger.Information("Macro Deck {Version}", Version);
        Logger.Information("Path: {ExecutablePath}", ApplicationPaths.ExecutablePath);
        Logger.Information("Start parameters: {StartParameters}",
            string.Join(" ", StartParameters.ToArray(StartParameters)));

        // 清理过期的日志文件
        MacroDeckLogger.CleanUpLogsDir();

        // 检查是否有待恢复的备份数据
        BackupManager.CheckRestoreDirectory();

        // 清理临时目录
        ApplicationPaths.CleanUpTempDirectory();

        // 加载语言资源文件
        LanguageManager.Load(StartParameters.ExportDefaultStrings);

        // 检查配置文件是否存在，不存在则进入初始设置向导
        if (!File.Exists(ApplicationPaths.MainConfigFilePath))
        {
            StartInitialSetup();
            return;
        }

        // 加载主配置文件
        Configuration = MainConfiguration.LoadFromFile(ApplicationPaths.MainConfigFilePath);
        SentryConfiguration.Enabled = Configuration.SendAnonymousErrorReports;
        LanguageManager.SetLanguage(Configuration.Language);
        FontManager.Initialize(Configuration.FontFamily);

        // 初始化热键管理器
        _ = new HotkeyManager();

        // 初始化变量管理器
        VariableManager.Initialize();

        // 加载所有插件
        PluginManager.Load();

        // 初始化图标管理器
        IconManager.Initialize();

        // 加载配置文件（包含按钮布局等）
        ProfileManager.Load();

        // 搜索并记录可用的网络接口
        SearchNetworkInterfaces();

        // 启动 WebSocket 服务器，优先使用启动参数指定的端口，否则使用配置中的端口
        MacroDeckServer.Start(StartParameters.Port <= 0 ? Configuration.HostPort : StartParameters.Port);

        // 启动广播服务器，用于局域网设备发现
        BroadcastServer.Start();

        // 异步初始化 ADB 服务器（用于 Android 设备连接）
        Task.Run(async () => await AdbServerHelper.Initialize());

        // 添加变量变化监听器和窗口焦点变化监听器
        ProfileManager.AddVariableChangedListener();
        ProfileManager.AddWindowFocusChangedListener();

        // 初始化命名管道服务器，用于单实例通信
        MacroDeckPipeServer.Initialize();
        MacroDeckPipeServer.PipeMessage += MacroDeckPipeServer_PipeMessage;

        // 设置系统托盘图标的右键菜单和事件处理
        TrayIcon.SetupTrayIcon(TrayIconContextMenu,
            ShowMainWindow,
            () => RestartMacroDeck(string.Join(" ", StartParameters)),
            Exit);

        // 创建并显示主窗口
        using (_mainWindow = new MainWindow())
        {
            // 捕获 UI 线程的同步上下文，供后续跨线程调用使用
            SyncContext = SynchronizationContext.Current;
        }

        StartUpTimeStopWatch.Stop();

        var startTook = StartUpTimeStopWatch.Elapsed.TotalMilliseconds;
        Logger.Information("Macro Deck startup finished (took {StartupDurationMs}ms)", startTook);

        // 触发加载完成事件
        OnMacroDeckLoaded?.Invoke(null, EventArgs.Empty);

        // 启动定期更新检查
        UpdateService.Instance().StartPeriodicalUpdateCheck();
        UpdateService.Instance().UpdateAvailable += OnUpdateAvailable;

        // 异步搜索扩展商店中的插件更新
        ExtensionStoreHelper.SearchUpdatesAsync();

        // 如果启动参数指定了显示主窗口，则显示
        if (StartParameters.ShowMainWindow)
        {
            ShowMainWindow();
        }

        // 启动 Windows 消息循环
        Application.Run();
    }

    /// <summary>
    /// 当检测到有可用更新时，显示系统通知
    /// </summary>
    private static void OnUpdateAvailable(object? sender, UpdateApiVersionInfo e)
    {
        var btnOpenSettings = new ButtonPrimary
        {
            AutoSize = true,
            Text = LanguageManager.Strings.OpenSettings
        };
        btnOpenSettings.Click += (o, args) => { MainWindow?.SetView(new SettingsView(2)); };

        NotificationManager.SystemNotification("Macro Deck Updater",
            string.Format(LanguageManager.Strings.VersionXIsNowAvailable,
                e.Version,
                e.IsBeta == true ? "Beta" : "Release"),
            true,
            new List<Control> { btnOpenSettings },
            Resources.Macro_Deck_2021_update);
    }

    /// <summary>
    /// 搜索并列出所有可用的 IPv4 网络接口，记录到日志中
    /// </summary>
    private static void SearchNetworkInterfaces()
    {
        StringBuilder sb = new();
        var foundNetworkInterfaces = 0;
        try
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 获取每个网络适配器的 IPv4 地址
                var address = adapter
                    .GetIPProperties()
                    .UnicastAddresses
                    .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork)?
                    .Address?
                    .ToString();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    sb.AppendLine($"{adapter.Name} - {address}");
                    foundNetworkInterfaces++;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Error while searching for network interfaces");
        }

        if (foundNetworkInterfaces == 0)
        {
            Logger.Error("No network interfaces were found");
        }
        else
        {
            Logger.Information("Found network interfaces:\n{NetworkInterfaces}", sb);
        }
    }

    /// <summary>
    /// 启动初始设置向导，当首次运行（无配置文件）时调用。
    /// 设置完成后保存配置并重启应用程序。
    /// </summary>
    private static void StartInitialSetup()
    {
        Logger.Information("Entering initial setup wizard...");
        using var initialSetup = new InitialSetup();
        Application.Run(initialSetup);
        if (initialSetup.DialogResult == DialogResult.OK)
        {
            // 用户完成初始设置，保存配置
            Configuration = initialSetup.configuration;
            Configuration.Save(ApplicationPaths.MainConfigFilePath);

            // 显示 Windows 防火墙提醒对话框
            using var defenderFirewallAlertInfo = new DefenderFirewallAlert();
            defenderFirewallAlertInfo.ShowDialog();

            // 重启应用程序以加载完整配置
            RestartMacroDeck("--show");
        }
        else
        {
            // 用户取消初始设置，退出程序
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// 处理命名管道消息，支持从另一个实例发送的命令
    /// </summary>
    /// <param name="message">管道消息内容，如 "show" 表示显示主窗口</param>
    private static void MacroDeckPipeServer_PipeMessage(string message)
    {
        switch (message)
        {
            case "show":
                ShowMainWindow();
                break;
        }
    }

    /// <summary>
    /// 在系统托盘图标上显示气泡提示通知
    /// </summary>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知内容</param>
    internal static void ShowBalloonTip(string title, string message)
    {
        try
        {
            TrayIcon?.ShowBalloonTip(5000, title, message, ToolTipIcon.Info);
        }
        catch
        {
            // 忽略托盘通知显示失败
        }
    }

    /// <summary>
    /// 请求重启 Macro Deck，弹出确认对话框
    /// </summary>
    public static void RequestRestart()
    {
        using var msgBox = new GUI.CustomControls.MessageBox();
        if (msgBox.ShowDialog(LanguageManager.Strings.MacroDeckNeedsARestart,
                LanguageManager.Strings.MacroDeckMustBeRestartedForTheChanges,
                MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            RestartMacroDeck();
        }
    }

    /// <summary>
    /// 重启 Macro Deck 应用程序。
    /// 启动新进程后退出当前进程，通过 --ignore-pid-check 参数避免单实例检查冲突。
    /// </summary>
    /// <param name="parameters">传递给新进程的额外启动参数</param>
    public static void RestartMacroDeck(string parameters = "")
    {
        TrayIcon.Visible = false;

        // 构建启动参数：如果当前主窗口可见则添加 --show 参数，并添加忽略 PID 检查参数
        var arguments = (_mainWindow is { IsDisposed: false } ? "--show " : "") +
            parameters +
            $" --ignore-pid-check {Process.GetCurrentProcess().Id}";
        Logger.Information("Restart Macro Deck with arguments: {Arguments}", arguments);
        var p = new Process
        {
            StartInfo = new ProcessStartInfo(ApplicationPaths.ExecutablePath)
            {
                UseShellExecute = true,
                Arguments = arguments
            }
        };
        p.Start();
        Environment.Exit(0);
    }

    /// <summary>
    /// 显示主窗口，确保在 UI 线程上执行创建操作
    /// </summary>
    public static void ShowMainWindow()
    {
        if (SyncContext is null)
        {
            CreateMainForm();
            return;
        }

        // 通过同步上下文将操作调度到 UI 线程
        SyncContext.Send(_ => { CreateMainForm(); }, null);
    }

    /// <summary>
    /// 创建或激活主窗口。
    /// 如果主窗口已存在则将其前置显示，否则创建新的主窗口实例。
    /// </summary>
    private static void CreateMainForm()
    {
        // 如果主窗口已存在且可用，则激活并前置显示
        if (Application.OpenForms.OfType<MainWindow>().Any() &&
            _mainWindow is { IsDisposed: false, IsHandleCreated: true })
        {
            if (_mainWindow.InvokeRequired)
            {
                _mainWindow.Invoke(ShowMainWindow);
                return;
            }

            _mainWindow.WindowState = FormWindowState.Minimized;
            _mainWindow.Show();
            _mainWindow.WindowState = FormWindowState.Normal;
            return;
        }

        // 创建新的主窗口
        _mainWindow = new MainWindow();
        _mainWindow.Load += MainWindowLoadEvent;
        _mainWindow.FormClosed += MainWindow_FormClosed;
        _mainWindow.Show();
    }

    /// <summary>
    /// 主窗口关闭事件处理，解除事件绑定并释放资源
    /// </summary>
    private static void MainWindow_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (_mainWindow == null)
        {
            return;
        }

        _mainWindow.Load -= MainWindowLoadEvent;
        _mainWindow.FormClosed -= MainWindow_FormClosed;
        _mainWindow.Dispose();
    }

    /// <summary>
    /// 主窗口加载完成事件处理，触发 OnMainWindowLoad 事件
    /// </summary>
    private static void MainWindowLoadEvent(object? sender, EventArgs e)
    {
        if (sender is MainWindow window)
        {
            OnMainWindowLoad?.Invoke(window, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 退出 Macro Deck 应用程序，先停止 ADB 服务器再退出
    /// </summary>
    public static void Exit()
    {
        AdbServerHelper.Stop().GetAwaiter().GetResult();
        Application.Exit();
    }
}
