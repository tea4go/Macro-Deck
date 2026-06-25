using System.Diagnostics;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using SuchByte.MacroDeck.Backup;
using SuchByte.MacroDeck.GUI.CustomControls.Settings;
using Serilog;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Services;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 设置视图，提供常规、连接、更新、备份、关于等配置页面。
/// </summary>
public partial class SettingsView : UserControl
{
    private static readonly ILogger Logger = Log.ForContext(typeof(SettingsView));

    /// <summary>
    /// 初始化设置视图，注册备份事件并加载文本翻译。
    /// </summary>
    /// <param name="page">初始显示的选项卡索引，默认为 0。</param>
    public SettingsView(int page = 0)
    {
        InitializeComponent();
        verticalTabControl.SelectTab(page);
        if (!DesignMode)
        {
            verticalTabControl.SelectedTabColor = Colors.AccentColor;
        }

        Dock = DockStyle.Fill;
        UpdateTranslation();
        BackupManager.BackupSaved += BackupManager_BackupSaved;
        BackupManager.BackupFailed += BackupManager_BackupFailed;
        BackupManager.DeleteSuccess += BackupManager_DeleteSuccess;
    }

    /// <summary>
    /// 检查是否有可用更新，并设置更新选项卡的通知标记。
    /// </summary>
    private void UpdateAvailable(object sender, EventArgs e)
    {
        verticalTabControl.SetNotification(2, UpdateService.Instance().VersionInfo != null);
    }

    /// <summary>
    /// 更新界面文本翻译，将所有 UI 文本设置为当前语言。
    /// </summary>
    private void UpdateTranslation()
    {
        Name = LanguageManager.Strings.SettingsTitle;
        tabGeneral.Text = LanguageManager.Strings.General;
        tabConnection.Text = LanguageManager.Strings.Connection;
        tabUpdater.Text = LanguageManager.Strings.Updates;
        tabAbout.Text = LanguageManager.Strings.About;
        lblGeneral.Text = LanguageManager.Strings.General;
        lblBehaviour.Text = LanguageManager.Strings.Behaviour;
        checkStartWindows.Text = LanguageManager.Strings.AutomaticallyStartWithWindows;
        checkSendErrorReports.Text = LanguageManager.Strings.SendAnonymousErrorReports;
        lblLanguage.Text = LanguageManager.Strings.Language;
        lblFont.Text = LanguageManager.Strings.Font;
        lblFontSize.Text = LanguageManager.Strings.Size;
        checkBold.Text = LanguageManager.Strings.Bold;
        lblConnection.Text = LanguageManager.Strings.Connection;
        lblPort.Text = LanguageManager.Strings.Port;
        btnChangePort.Text = LanguageManager.Strings.Ok;
        lblUpdates.Text = LanguageManager.Strings.Updates;
        checkAutoUpdate.Text = LanguageManager.Strings.AutomaticallyCheckUpdates;
        lblInstalledVersionLabel.Text = LanguageManager.Strings.InstalledVersion;
        tabBackups.Text = LanguageManager.Strings.Backups;
        lblBackups.Text = LanguageManager.Strings.Backups;
        btnCreateBackup.Text = LanguageManager.Strings.CreateBackup;
        checkInstallBetaVersions.Text = LanguageManager.Strings.InstallBetaVersions;
        btnCheckUpdates.Text = LanguageManager.Strings.CheckForUpdatesNow;
        lblWebSocketAPILabel.Text = LanguageManager.Strings.WebSocketAPIVersion;
        lblPluginAPILabel.Text = LanguageManager.Strings.PluginAPIVersion;
        lblInstalledPluginsLabel.Text = LanguageManager.Strings.InstalledPlugins;
        lblTranslationBy.Text = string.Format(LanguageManager.Strings.XTranslationByX,
            LanguageManager.Strings.__Language__,
            LanguageManager.Strings.__Author__);
        checkEnableSsl.Text = LanguageManager.Strings.EnableSsl;
        btnApplySslConfiguration.Text = LanguageManager.Strings.ApplySslConfiguration;
        btnGenerateCertificate.Text = LanguageManager.Strings.GenerateNewCertificate;
        label4.Text = LanguageManager.Strings.PrivateKeyPem;
        label3.Text = LanguageManager.Strings.CertificatePem;
        checkEnableAdb.Text = LanguageManager.Strings.EnableAdb;
        checkAutoStartUsb.Text = LanguageManager.Strings.AutoWakeStartClient;
        label6.Text = LanguageManager.Strings.RuntimeLabel;
        label1.Text = LanguageManager.Strings.LicensedUnderApache;
    }

    /// <summary>
    /// 设置页面加载时初始化所有配置项的显示。
    /// </summary>
    private void Settings_Load(object sender, EventArgs e)
    {
        LoadAutoStart();
        LoadErrorReporting();
        LoadLanguage();
        LoadFont();
        LoadUpdateChannel();
        LoadNetworkConfiguration();
        LoadAutoUpdate();
        LoadBackups();

        lblInstalledVersion.Text = MacroDeck.Version.ToString();
        lblWebsocketAPIVersion.Text = MacroDeck.ApiVersion.ToString();
        lblPluginAPIVersion.Text = MacroDeck.PluginApiVersion.ToString();
        lblMacroDeck.Text = "Macro Deck " + MacroDeck.Version.ToString();
        lblInstalledPlugins.Text = PluginManager.Plugins.Count.ToString();
        lblDotnetVersion.Text = RuntimeInformation.FrameworkDescription;
    }

    /// <summary>
    /// 加载语言列表到下拉框，并根据当前配置设置选中项。
    /// </summary>
    private void LoadLanguage()
    {
        language.SelectedIndexChanged -= Language_SelectedIndexChanged;
        language.Items.Clear();
        foreach (var strings in LanguageManager.Languages)
        {
            language.Items.Add(strings.__Language__);
        }

        language.Text = MacroDeck.Configuration.Language;
        language.SelectedIndexChanged += Language_SelectedIndexChanged;
    }

    /// <summary>
    /// 加载系统字体列表到下拉框，并根据当前配置设置字体相关选项。
    /// </summary>
    private void LoadFont()
    {
        font.SelectedIndexChanged -= Font_SelectedIndexChanged;
        fontSize.ValueChanged -= FontSize_ValueChanged;
        checkBold.CheckedChanged -= CheckBold_CheckedChanged;

        font.Items.Clear();
        using (var col = new InstalledFontCollection())
        {
            foreach (var fontFamily in col.Families)
            {
                font.Items.Add(fontFamily.Name);
            }
        }

        font.Text = MacroDeck.Configuration.FontFamily;

        var size = (decimal)MacroDeck.Configuration.FontSize;
        if (size < fontSize.Minimum) size = fontSize.Minimum;
        if (size > fontSize.Maximum) size = fontSize.Maximum;
        fontSize.Value = size;

        checkBold.Checked = MacroDeck.Configuration.FontBold;

        font.SelectedIndexChanged += Font_SelectedIndexChanged;
        fontSize.ValueChanged += FontSize_ValueChanged;
        checkBold.CheckedChanged += CheckBold_CheckedChanged;
    }

    /// <summary>
    /// 加载自动更新设置到复选框。
    /// </summary>
    private void LoadAutoUpdate()
    {
        checkAutoUpdate.CheckedChanged -= CheckAutoUpdate_CheckedChanged;
        checkAutoUpdate.Checked = MacroDeck.Configuration.AutoUpdates;
        checkAutoUpdate.CheckedChanged += CheckAutoUpdate_CheckedChanged;
    }

    /// <summary>
    /// 加载网络配置（端口、SSL、ADB 等）到对应控件。
    /// </summary>
    private void LoadNetworkConfiguration()
    {
        port.Value = MacroDeck.Configuration.HostPort;
        checkEnableSsl.Checked = MacroDeck.Configuration.EnableSsl;
        certPemTextBox.Text = MacroDeck.Configuration.SslCertificatePem ?? string.Empty;
        keyPemTextBox.Text = string.IsNullOrEmpty(MacroDeck.Configuration.SslCertificateKeyPemEncrypted)
            ? string.Empty
            : "(Key saved – paste new key to replace)";
        checkEnableAdb.Checked = MacroDeck.Configuration.EnableAdbServer;
        checkAutoStartUsb.Checked = MacroDeck.Configuration.EnableAdbAutoStartApp;
        checkEnableAdb.CheckedChanged += CheckEnableAdb_CheckedChanged;
        checkAutoStartUsb.CheckedChanged += CheckAutoStartUsb_CheckedChanged;
    }

    /// <summary>
    /// ADB 自动启动客户端选项变更时保存配置。
    /// </summary>
    private void CheckAutoStartUsb_CheckedChanged(object? sender, EventArgs e)
    {
        MacroDeck.Configuration.EnableAdbAutoStartApp = checkAutoStartUsb.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }

    /// <summary>
    /// ADB 服务启用选项变更时保存配置。
    /// </summary>
    private void CheckEnableAdb_CheckedChanged(object? sender, EventArgs e)
    {
        MacroDeck.Configuration.EnableAdbServer = checkEnableAdb.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }

    /// <summary>
    /// 加载开机自启动设置到复选框。
    /// </summary>
    private void LoadAutoStart()
    {
        checkStartWindows.CheckedChanged -= CheckStartWindows_CheckedChanged;
        checkStartWindows.Checked = MacroDeck.Configuration.AutoStart;
        checkStartWindows.CheckedChanged += CheckStartWindows_CheckedChanged;
    }

    /// <summary>
    /// 加载匿名错误报告设置到复选框。
    /// </summary>
    private void LoadErrorReporting()
    {
        checkSendErrorReports.CheckedChanged -= CheckSendErrorReports_CheckedChanged;
        checkSendErrorReports.Checked = MacroDeck.Configuration.SendAnonymousErrorReports;
        checkSendErrorReports.CheckedChanged += CheckSendErrorReports_CheckedChanged;
    }

    /// <summary>
    /// 匿名错误报告选项变更时，更新 Sentry 配置并保存。
    /// </summary>
    private void CheckSendErrorReports_CheckedChanged(object sender, EventArgs e)
    {
        MacroDeck.Configuration.SendAnonymousErrorReports = checkSendErrorReports.Checked;
        SentryConfiguration.Enabled = checkSendErrorReports.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }

    /// <summary>
    /// 加载测试版更新渠道设置到复选框。
    /// </summary>
    private void LoadUpdateChannel()
    {
        checkInstallBetaVersions.CheckedChanged -= CheckInstallBetaVersions_CheckedChanged;
        checkInstallBetaVersions.Checked = MacroDeck.Configuration.UpdateBetaVersions;
        checkInstallBetaVersions.CheckedChanged += CheckInstallBetaVersions_CheckedChanged;
    }

    /// <summary>
    /// 加载备份列表到面板。
    /// </summary>
    private void LoadBackups()
    {
        backupsPanel.Controls.Clear();
        foreach (var macroDeckBackupInfo in BackupManager.GetBackups().ToArray())
        {
            var backupItem = new BackupItem(macroDeckBackupInfo);
            backupsPanel.Controls.Add(backupItem);
        }
    }

    /// <summary>
    /// 测试版更新渠道选项变更时，弹出警告确认后保存配置。
    /// </summary>
    private void CheckInstallBetaVersions_CheckedChanged(object sender, EventArgs e)
    {
        if (checkInstallBetaVersions.Checked)
        {
            using var msgBox = new MessageBox();
            if (msgBox.ShowDialog(LanguageManager.Strings.Warning,
                    LanguageManager.Strings.WarningBetaVersions,
                    MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                MacroDeck.Configuration.UpdateBetaVersions = true;
                MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
            }
            else
            {
                LoadUpdateChannel();
            }
        }
        else
        {
            MacroDeck.Configuration.UpdateBetaVersions = false;
            MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
        }
    }

    /// <summary>
    /// 修改端口号后保存配置并请求重启应用。
    /// </summary>
    private void BtnChangePort_Click(object sender, EventArgs e)
    {
        if (port.Value == MacroDeck.Configuration.HostPort)
        {
            return;
        }

        MacroDeck.Configuration.HostPort = (int)port.Value;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
        MacroDeck.RequestRestart();
    }

    /// <summary>
    /// 开机自启动选项变更时保存配置。
    /// </summary>
    private void CheckStartWindows_CheckedChanged(object sender, EventArgs e)
    {
        MacroDeck.Configuration.AutoStart = checkStartWindows.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }

    /// <summary>
    /// 自动更新选项变更时保存配置。
    /// </summary>
    private void CheckAutoUpdate_CheckedChanged(object sender, EventArgs e)
    {
        MacroDeck.Configuration.AutoUpdates = checkAutoUpdate.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
    }

    /// <summary>
    /// 手动检查更新，显示加载动画，根据结果弹出对话框。
    /// </summary>
    private async void BtnCheckUpdates_Click(object sender, EventArgs e)
    {
        btnCheckUpdates.Enabled = false;
        btnCheckUpdates.Spinner = true;

        try
        {
            var availableUpdate = await UpdateService.Instance().CheckForUpdatesAsync(CancellationToken.None);
            if (availableUpdate == null)
            {
                using var msgBox = new MessageBox();
                msgBox.ShowDialog(LanguageManager.Strings.NoUpdatesAvailable,
                    LanguageManager.Strings.LatestVersionInstalled,
                    MessageBoxButtons.OK);

                return;
            }

            using var updateAvailableDialog = new UpdateAvailableDialog(availableUpdate);
            updateAvailableDialog.ShowDialog();
        }
        catch (Exception ex)
        {
            using var msgBox = new MessageBox();
            msgBox.ShowDialog("Check for updates failed",
                "Make sure you have a internet connection",
                MessageBoxButtons.OK);

            Logger.Error(ex, "Failed to check for updates");
        }
        finally
        {
            Invoke(() =>
            {
                btnCheckUpdates.Enabled = true;
                btnCheckUpdates.Spinner = false;
            });
        }
    }

    /// <summary>
    /// 打开开源许可证对话框。
    /// </summary>
    private void BtnLicenses_Click(object sender, EventArgs e)
    {
        using var licensesDialog = new LicensesDialog();
        licensesDialog.ShowDialog();
    }

    /// <summary>
    /// 语言选择变更时，保存配置并刷新界面翻译。
    /// </summary>
    private void Language_SelectedIndexChanged(object sender, EventArgs e)
    {
        MacroDeck.Configuration.Language = language.Text;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
        LanguageManager.SetLanguage(MacroDeck.Configuration.Language);
        UpdateTranslation();
    }

    /// <summary>
    /// 字体选择变更时保存配置并提示重启。
    /// </summary>
    private void Font_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (font.Text == MacroDeck.Configuration.FontFamily)
        {
            return;
        }

        MacroDeck.Configuration.FontFamily = font.Text;
        SaveFontAndNotifyRestart();
    }

    /// <summary>
    /// 字体大小变更时保存配置并提示重启。
    /// </summary>
    private void FontSize_ValueChanged(object sender, EventArgs e)
    {
        var value = (float)fontSize.Value;
        if (Math.Abs(value - MacroDeck.Configuration.FontSize) < 0.001f)
        {
            return;
        }

        MacroDeck.Configuration.FontSize = value;
        SaveFontAndNotifyRestart();
    }

    /// <summary>
    /// 粗体选项变更时保存配置并提示重启。
    /// </summary>
    private void CheckBold_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBold.Checked == MacroDeck.Configuration.FontBold)
        {
            return;
        }

        MacroDeck.Configuration.FontBold = checkBold.Checked;
        SaveFontAndNotifyRestart();
    }

    /// <summary>
    /// 保存字体配置，实时刷新所有打开窗体的字体，并显示提示信息。
    /// 自定义绘制控件需重启后完全生效。
    /// </summary>
    private void SaveFontAndNotifyRestart()
    {
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);

        // 实时刷新所有已打开窗体的字体（自定义绘制部分仍需重启完全生效）
        FontManager.UpdateAndRefresh(MacroDeck.Configuration.FontFamily,
            MacroDeck.Configuration.FontSize,
            MacroDeck.Configuration.FontBold);

        lblFontRestartHint.Text = LanguageManager.Strings.FontPartialRestartHint;
        lblFontRestartHint.Visible = true;
    }

    /// <summary>
    /// 备份失败时停止加载动画并弹出错误提示。
    /// </summary>
    private void BackupManager_BackupFailed(object sender, BackupFailedEventArgs e)
    {
        Invoke(() =>
        {
            btnCreateBackup.Spinner = false;
            using var msgBox = new MessageBox();
            msgBox.ShowDialog(LanguageManager.Strings.Backup,
                LanguageManager.Strings.BackupFailed + ": " + Environment.NewLine + e.Message,
                MessageBoxButtons.OK);
        });
    }

    /// <summary>
    /// 备份成功时停止加载动画，弹出提示并刷新备份列表。
    /// </summary>
    private void BackupManager_BackupSaved(object sender, EventArgs e)
    {
        Invoke(() =>
        {
            btnCreateBackup.Spinner = false;
            using (var msgBox = new MessageBox())
            {
                msgBox.ShowDialog(LanguageManager.Strings.Backup,
                    LanguageManager.Strings.BackupSuccessfullyCreated,
                    MessageBoxButtons.OK);
            }

            LoadBackups();
        });
    }

    /// <summary>
    /// 点击"创建备份"按钮，启动加载动画并异步创建备份。
    /// </summary>
    private void BtnCreateBackup_Click(object sender, EventArgs e)
    {
        btnCreateBackup.Spinner = true;
        Task.Run(() => { BackupManager.CreateBackup(); });
    }

    /// <summary>
    /// 备份删除成功后刷新备份列表。
    /// </summary>
    private void BackupManager_DeleteSuccess(object sender, EventArgs e)
    {
        LoadBackups();
    }

    /// <summary>
    /// 打开 GitHub 项目页面。
    /// </summary>
    private void BtnGitHub_Click(object sender, EventArgs e)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("https://github.com/Macro-Deck-App/Macro-Deck")
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// 应用 SSL 配置：校验证书和密钥，保存配置并请求重启。
    /// </summary>
    private void BtnApplySslConfiguration_Click(object sender, EventArgs e)
    {
        if (checkEnableSsl.Checked)
        {
            var certPem = certPemTextBox.Text.Trim();
            var keyPemInput = keyPemTextBox.Text.Trim();
            var isKeyPlaceholder = keyPemInput.StartsWith("(Key saved");

            if (string.IsNullOrWhiteSpace(certPem))
            {
                using var msgBox = new MessageBox();
                msgBox.ShowDialog("SSL Configuration",
                    "Please provide a certificate in PEM format.",
                    MessageBoxButtons.OK);
                return;
            }

            if (!isKeyPlaceholder)
            {
                if (!SslCertificateService.TryValidate(certPem, keyPemInput, out var validationError))
                {
                    using var msgBox = new MessageBox();
                    msgBox.ShowDialog("Invalid SSL Certificate",
                        validationError ?? "Certificate and key do not match.",
                        MessageBoxButtons.OK);
                    return;
                }

                SslCertificateService.SaveCertificate(certPem, keyPemInput);
            }
            else
            {
                MacroDeck.Configuration.SslCertificatePem = certPem;
                MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
            }
        }

        MacroDeck.Configuration.EnableSsl = checkEnableSsl.Checked;
        MacroDeck.Configuration.Save(ApplicationPaths.MainConfigFilePath);
        MacroDeck.RequestRestart();
    }

    /// <summary>
    /// 生成自签名 SSL 证书，异步生成后填充到文本框中。
    /// </summary>
    private void BtnGenerateCertificate_Click(object sender, EventArgs e)
    {
        btnGenerateCertificate.Spinner = true;
        btnGenerateCertificate.Enabled = false;
        Task.Run(() =>
        {
            var (certPem, keyPem) = SelfSignedCertificateGenerator.Generate();
            Invoke(() =>
            {
                certPemTextBox.Text = certPem;
                keyPemTextBox.Text = keyPem;
                btnGenerateCertificate.Spinner = false;
                btnGenerateCertificate.Enabled = true;
            });
        });
    }
}
