using SuchByte.MacroDeck.DataTypes.Updater;
using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.GUI.MainWindowViews;
using SuchByte.MacroDeck.Icons;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Notifications;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.Services;
using System.Diagnostics;
using SuchByte.MacroDeck.GUI.CustomControls.Notifications;
using Form = SuchByte.MacroDeck.GUI.CustomControls.Form;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.GUI;

/// <summary>
/// Macro Deck 主窗口类。
/// 作为应用程序的主界面，负责管理各个功能视图（Deck、设备管理器、扩展、设置、变量）的切换和显示，
/// 处理通知、更新检查、插件变更等系统事件，并提供导航栏和顶部状态栏的交互功能。
/// </summary>
public partial class MainWindow : Form
{
    /// <summary>通知列表面板，用于显示系统通知的弹出列表</summary>
    private NotificationsList? _notificationsList;

    /// <summary>Deck 视图实例，显示动作按钮网格的主界面</summary>
    private DeckView? _deckView;

    /// <summary>
    /// 获取 Deck 视图实例。
    /// 如果实例不存在、已释放或句柄未创建，则重新创建一个新的 DeckView 实例。
    /// 这种延迟初始化模式确保了视图在需要时才被创建，避免不必要的资源占用。
    /// </summary>
    public DeckView DeckView
    {
        get
        {
            if (_deckView is null || _deckView.IsDisposed || !_deckView.IsHandleCreated)
            {
                _deckView = new DeckView(this);
            }

            return _deckView;
        }
    }


    /// <summary>
    /// 初始化主窗口。
    /// 设置通知按钮背景透明，订阅语言变更、更新可用、窗口显示等事件，
    /// 并创建初始的 DeckView 实例。
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        btnNotifications.BackColor = Color.Transparent;
        UpdateTranslation();
        LanguageManager.LanguageChanged += LanguageChanged;
        UpdateService.Instance().UpdateAvailable += UpdateAvailable;
        Shown += MainWindowShown;

        _deckView = new DeckView(this);
    }

    /// <summary>
    /// 当检测到新版本可用时显示更新对话框。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">包含更新版本信息的参数</param>
    private void UpdateAvailable(object? sender, UpdateApiVersionInfo e)
    {
        using var updateAvailableDialog = new UpdateAvailableDialog(e);
        updateAvailableDialog.ShowDialog();
    }

    /// <summary>
    /// 更新界面文本翻译。
    /// 当前为空实现，文本翻译由各子视图自行处理。
    /// </summary>
    private void UpdateTranslation()
    {
    }

    /// <summary>
    /// 当系统语言变更时触发。
    /// 更新主窗口和 Deck 视图的文本翻译。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void LanguageChanged(object? sender, EventArgs e)
    {
        UpdateTranslation();
        DeckView?.UpdateTranslation();
    }

    /// <summary>
    /// 选中指定的内容导航按钮，取消其他按钮的选中状态。
    /// 用于在视图切换时同步导航栏的视觉状态。
    /// </summary>
    /// <param name="control">要选中的导航按钮控件</param>
    public void SelectContentButton(Control control)
    {
        foreach (var contentButton in contentButtonPanel.Controls.OfType<ContentSelectorButton>()
            .Where(x => x != control && x.Selected))
        {
            contentButton.Selected = false;
        }

        btnSettings.Selected = false;
        ((ContentSelectorButton)control).Selected = true;
    }

    /// <summary>
    /// 设置主窗口当前显示的视图。
    /// 清理并移除当前内容面板中除 DeckView 外的其他视图，然后添加并显示新视图。
    /// 根据视图类型自动选中对应的导航按钮。
    /// </summary>
    /// <param name="view">要显示的视图控件</param>
    public void SetView(Control view)
    {
        // 如果视图已在内容面板中，则无需重复添加
        if (contentPanel.Controls.Contains(view))
        {
            return;
        }

        // 释放除新视图和 DeckView 外的所有控件资源
        foreach (var control in contentPanel.Controls.OfType<Control>().Where(x => x != view && x != DeckView))
        {
            control.Dispose();
        }

        // 从内容面板中移除非新视图的控件
        foreach (var control in contentPanel.Controls.OfType<Control>().Where(x => x != view))
        {
            contentPanel.Controls.Remove(control);
        }

        // 添加新视图到内容面板
        contentPanel.Controls.Add(view);

        // 根据视图类型选中对应的导航按钮
        switch (view)
        {
            case DeckView _:
                SelectContentButton(btnDeck);
                break;
            case DeviceManagerView:
                SelectContentButton(btnDeviceManager);
                break;
            case ExtensionsView:
                SelectContentButton(btnExtensions);
                break;
            case SettingsView:
                SelectContentButton(btnSettings);
                break;
            case VariablesView:
                SelectContentButton(btnVariables);
                break;
        }
    }

    /// <summary>
    /// 主窗口首次显示时触发的处理逻辑。
    /// 刷新插件标签，检查安全模式，设置默认显示 DeckView，
    /// 检查扩展商店更新，显示通知数量，并在有新版本时显示更新对话框。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void MainWindowShown(object? sender, EventArgs e)
    {
        Application.DoEvents();
        RefreshPluginsLabels();

        // 安全模式提示：背景变红，警告用户更改不会被保存
        if (MacroDeck.SafeMode)
        {
            BackColor = Color.FromArgb(99, 0, 0);
            using var msgBox = new MessageBox();
            msgBox.ShowDialog("Safe mode",
                "Macro Deck was started in safe mode! This means no changes on the action buttons will be saved to prevent damage.",
                MessageBoxButtons.OK);
        }

        // 设置默认显示 DeckView
        SetView(DeckView);

        // 根据版本信息显示设置按钮的通知标记
        btnSettings.SetNotification(UpdateService.Instance().VersionInfo != null);
        // 异步搜索扩展商店更新
        ExtensionStoreHelper.SearchUpdatesAsync();

        // 更新通知按钮显示的通知数量
        btnNotifications.NotificationCount = NotificationManager.Notifications.Count;

        // 如果有可用更新，显示更新对话框
        var updateApiVersionInfo = UpdateService.Instance().VersionInfo;
        if (updateApiVersionInfo != null)
        {
            using var updateAvailableDialog = new UpdateAvailableDialog(updateApiVersionInfo);
            updateAvailableDialog.ShowDialog();
        }
    }

    /// <summary>
    /// 主窗口加载时触发的处理逻辑。
    /// 设置版本标签文本，订阅插件变更、图标包变更、设备连接状态变更、通知变更等事件，
    /// 并将窗口居中显示。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void MainWindow_Load(object? sender, EventArgs e)
    {
        lblVersion.Text = $@"Macro Deck {MacroDeck.Version}";

        PluginManager.OnPluginsChange += OnPluginsChanged;
        IconManager.OnIconPacksChanged += OnPluginsChanged;
        IconManager.OnUpdateCheckFinished += OnPackageManagerUpdateCheckFinished;

        MacroDeckServer.OnDeviceConnectionStateChanged += OnClientsConnectedChanged;
        OnClientsConnectedChanged(null, EventArgs.Empty);

        NotificationManager.OnNotification += NotificationsChanged;
        NotificationManager.OnNotificationRemoved += NotificationsChanged;
        ExtensionStoreHelper.OnInstallationFinished += ExtensionStoreHelper_OnInstallationFinished;

        CenterToScreen();
    }

    /// <summary>
    /// 当通知列表变更时更新通知按钮显示的数量。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void NotificationsChanged(object? sender, EventArgs e)
    {
        btnNotifications.NotificationCount = NotificationManager.Notifications.Count;
    }

    /// <summary>
    /// 当扩展商店安装完成时刷新插件标签。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void ExtensionStoreHelper_OnInstallationFinished(object? sender, EventArgs e)
    {
        RefreshPluginsLabels();
    }

    /// <summary>
    /// 当包管理器更新检查完成时刷新插件标签。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void OnPackageManagerUpdateCheckFinished(object? sender, EventArgs e)
    {
        RefreshPluginsLabels();
    }

    /// <summary>
    /// 当插件列表变更时刷新插件标签。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void OnPluginsChanged(object? sender, EventArgs e)
    {
        RefreshPluginsLabels();
    }

    /// <summary>
    /// 刷新插件标签的通知状态。
    /// 如果有可用的插件更新或图标包更新，则在扩展按钮上显示通知标记。
    /// 使用 Invoke 确保在 UI 线程上执行更新操作。
    /// </summary>
    private void RefreshPluginsLabels()
    {
        // 如果窗口句柄未创建或已释放，则跳过更新
        if (!IsHandleCreated || IsDisposed)
        {
            return;
        }

        Invoke(() =>
        {
            btnExtensions.SetNotification(PluginManager.PluginsUpdateAvailable.Count > 0 ||
                IconManager.IconPacksUpdateAvailable.Count > 0);
        });
    }

    /// <summary>
    /// 当客户端连接状态变更时更新已连接客户端数量标签。
    /// 使用 Invoke 确保在 UI 线程上执行更新操作。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void OnClientsConnectedChanged(object? sender, EventArgs e)
    {
        Invoke(new Action(() =>
            lblNumClientsConnected.Text
                = string.Format(LanguageManager.Strings.XClientsConnected, MacroDeckServer.Clients.Count)));
    }

    /// <summary>
    /// Deck 按钮点击事件处理。
    /// 切换到 DeckView 并刷新按钮显示。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnDeck_Click(object? sender, EventArgs e)
    {
        SetView(DeckView);
        DeckView.UpdateButtons();
    }

    /// <summary>
    /// 扩展按钮点击事件处理。
    /// 切换到扩展商店视图。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnExtensions_Click(object? sender, EventArgs e)
    {
        SetView(new ExtensionsView());
    }

    /// <summary>
    /// 设置按钮点击事件处理。
    /// 切换到设置视图。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        SetView(new SettingsView());
    }

    /// <summary>
    /// 设备管理器按钮点击事件处理。
    /// 切换到设备管理器视图。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnDeviceManager_Click(object? sender, EventArgs e)
    {
        SetView(new DeviceManagerView());
    }

    /// <summary>
    /// 窗口关闭时清理内容面板中的所有控件资源。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    public void OnFormClosing(object? sender, EventArgs e)
    {
        foreach (Control control in contentPanel.Controls)
        {
            control.Dispose();
        }
    }

    /// <summary>
    /// 变量按钮点击事件处理。
    /// 切换到变量管理视图。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnVariables_Click(object? sender, EventArgs e)
    {
        SetView(new VariablesView());
    }

    /// <summary>
    /// 通知按钮点击事件处理。
    /// 显示或隐藏通知列表面板，面板位置相对于通知按钮定位。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void BtnNotifications_Click(object? sender, EventArgs e)
    {
        // 如果通知列表未创建或已释放，则创建新实例并设置位置
        if (_notificationsList == null || _notificationsList.IsDisposed)
        {
            _notificationsList = new NotificationsList
            {
                Location = btnNotifications.Location with
                {
                    Y = btnNotifications.Location.Y + btnNotifications.Height,
                    X = btnNotifications.Location.X + btnNotifications.Size.Width + 20
                }
            };
            _notificationsList.OnCloseRequested += (_, _) => { Controls.Remove(_notificationsList); };
        }

        // 切换通知列表的显示状态
        if (Controls.Contains(_notificationsList))
        {
            Controls.Remove(_notificationsList);
        }
        else
        {
            Controls.Add(_notificationsList);
            _notificationsList.BringToFront();
        }
    }

    /// <summary>
    /// 捐赠按钮点击事件处理。
    /// 在默认浏览器中打开 Ko-fi 捐赠页面。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void btnDonate_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("https://ko-fi.com/manuelmayer")
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// Discord 按钮点击事件处理。
    /// 在默认浏览器中打开 Macro Deck Discord 服务器链接。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">事件参数</param>
    private void btnDiscord_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("https://discord.macro-deck.app")
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }
}
