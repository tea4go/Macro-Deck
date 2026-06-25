using SuchByte.MacroDeck.GUI.CustomControls.ExtensionsView;
using SuchByte.MacroDeck.Language;

namespace SuchByte.MacroDeck.GUI.MainWindowViews;

/// <summary>
/// 扩展管理视图，用于管理已安装扩展、在线扩展商店和 ZIP 包安装。
/// </summary>
public partial class ExtensionsView : UserControl
{
    /// <summary>
    /// 在线扩展商店视图实例。
    /// </summary>
    private ExtensionStoreView extensionStoreView;
    /// <summary>
    /// ZIP 包安装器视图实例。
    /// </summary>
    private ExtensionZipInstallerView extensionZipInstallerView;
    /// <summary>
    /// 已安装扩展列表视图实例。
    /// </summary>
    private InstalledExtensionsView installedExtensionsView;

    /// <summary>
    /// 初始化扩展视图，设置默认文本翻译。
    /// </summary>
    public ExtensionsView()
    {
        InitializeComponent();
        Dock = DockStyle.Fill;
        Name = LanguageManager.Strings.Extensions;
        radioInstalled.Text = LanguageManager.Strings.Installed;
        radioOnline.Text = LanguageManager.Strings.Online;
    }

    /// <summary>
    /// 切换到在线扩展商店视图。
    /// </summary>
    private void SetExtensionStoreView()
    {
        if (extensionStoreView == null || extensionStoreView.IsDisposed)
        {
            extensionStoreView = new ExtensionStoreView();
        }

        content.Controls.Clear();
        content.Controls.Add(extensionStoreView);
    }

    /// <summary>
    /// 切换到 ZIP 包安装器视图。
    /// </summary>
    private void SetExtensionZipInstallerView()
    {
        if (extensionZipInstallerView == null || extensionZipInstallerView.IsDisposed)
        {
            extensionZipInstallerView = new ExtensionZipInstallerView();
            extensionZipInstallerView.RequestClose += ExtensionStoreView_RequestClose;
        }

        extensionZipInstallerView.Height = ClientRectangle.Height;
        extensionZipInstallerView.Width = ClientRectangle.Width;
        content.Controls.Clear();
        content.Controls.Add(extensionZipInstallerView);
    }

    /// <summary>
    /// 关闭 ZIP 安装器后返回已安装扩展列表视图。
    /// </summary>
    private void ExtensionStoreView_RequestClose(object sender, EventArgs e)
    {
        SetInstalledExtensionsView();
    }

    /// <summary>
    /// 切换到已安装扩展列表视图，并刷新扩展列表。
    /// </summary>
    private void SetInstalledExtensionsView()
    {
        if (installedExtensionsView == null)
        {
            installedExtensionsView = new InstalledExtensionsView();
            installedExtensionsView.RequestExtensionStore += InstalledExtensionsView_RequestExtensionStore;
            installedExtensionsView.RequestZipInstaller += InstalledExtensionsView_RequestZipInstaller;
        }

        extensionStoreView?.Dispose();
        content.Controls.Clear();
        content.Controls.Add(installedExtensionsView);
        installedExtensionsView.ListInstalledExtensions();
    }

    /// <summary>
    /// 响应用户请求，从已安装列表切换到 ZIP 安装器。
    /// </summary>
    private void InstalledExtensionsView_RequestZipInstaller(object sender, EventArgs e)
    {
        SetExtensionZipInstallerView();
    }

    /// <summary>
    /// 响应用户请求，从已安装列表切换到在线扩展商店。
    /// </summary>
    private void InstalledExtensionsView_RequestExtensionStore(object sender, EventArgs e)
    {
        SetExtensionStoreView();
    }

    /// <summary>
    /// 扩展视图加载时，默认显示已安装扩展列表。
    /// </summary>
    private void ExtensionStoreView_Load(object sender, EventArgs e)
    {
        SetInstalledExtensionsView();
    }

    /// <summary>
    /// 选择"已安装"选项卡时切换到已安装扩展视图。
    /// </summary>
    private void RadioInstalled_CheckedChanged(object sender, EventArgs e)
    {
        if (radioInstalled.Checked)
        {
            SetInstalledExtensionsView();
        }
    }

    /// <summary>
    /// 选择"在线"选项卡时切换到在线扩展商店视图。
    /// </summary>
    private void RadioOnline_CheckedChanged(object sender, EventArgs e)
    {
        if (radioOnline.Checked)
        {
            SetExtensionStoreView();
        }
    }
}
