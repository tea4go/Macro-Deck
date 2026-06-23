using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.GUI.MainWindowViews;
using SuchByte.MacroDeck.Icons;
using Serilog;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Models;
using SuchByte.MacroDeck.Notifications;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Properties;

namespace SuchByte.MacroDeck.ExtensionStore;

/// <summary>
/// 扩展商店辅助类，提供插件和图标包的安装、更新检查和批量更新功能。
/// 通过扩展商店 API 获取最新版本信息，并使用 ExtensionStoreDownloader 进行下载安装。
/// </summary>
public class ExtensionStoreHelper
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ExtensionStoreHelper));

    /// <summary>更新检查完成事件</summary>
    public static event EventHandler OnUpdateCheckFinished;

    /// <summary>扩展商店下载器实例</summary>
    private static ExtensionStoreDownloader extensionStoreDownloader;

    /// <summary>安装完成事件</summary>
    public static event EventHandler OnInstallationFinished;

    /// <summary>更新通知标识</summary>
    private static string _updateNotification;

    /// <summary>是否正在执行更新检查</summary>
    private static bool _updateCheckRunning;

    /// <summary>
    /// 通过包标识安装插件
    /// </summary>
    /// <param name="packageId">插件包标识</param>
    public static void InstallPluginById(string packageId)
    {
        InstallPackages(new List<ExtensionStoreDownloaderPackageInfoModel>
            { new() { PackageId = packageId, ExtensionType = ExtensionType.Plugin } });
    }

    /// <summary>
    /// 通过包标识安装图标包
    /// </summary>
    /// <param name="packageId">图标包包标识</param>
    public static void InstallIconPackById(string packageId)
    {
        InstallPackages(new List<ExtensionStoreDownloaderPackageInfoModel>
            { new() { PackageId = packageId, ExtensionType = ExtensionType.IconPack } });
    }

    /// <summary>
    /// 通过包标识安装扩展（内部方法，自动判断类型）
    /// </summary>
    /// <param name="packageId">扩展包标识</param>
    internal static void InstallById(string packageId)
    {
        InstallPackages(new List<ExtensionStoreDownloaderPackageInfoModel> { new() { PackageId = packageId } });
    }

    /// <summary>
    /// 安装指定的扩展包列表。弹出扩展商店下载器对话框进行下载安装。
    /// </summary>
    /// <param name="packages">要安装的扩展包信息列表</param>
    public static void InstallPackages(List<ExtensionStoreDownloaderPackageInfoModel> packages)
    {
        if (MacroDeck.MainWindow == null)
        {
            return;
        }

        MacroDeck.MainWindow.Invoke(() =>
        {
            extensionStoreDownloader = new ExtensionStoreDownloader(packages)
            {
                Owner = MacroDeck.MainWindow
            };
            extensionStoreDownloader.ShowDialog();
            OnInstallationFinished?.Invoke(null, EventArgs.Empty);
        });
    }

    /// <summary>
    /// 获取插件的包标识（基于插件目录名）
    /// </summary>
    /// <param name="macroDeckPlugin">目标插件</param>
    /// <returns>包标识字符串</returns>
    public static string GetPackageId(MacroDeckPlugin macroDeckPlugin)
    {
        return new DirectoryInfo(PluginManager.PluginDirectories[macroDeckPlugin]).Name;
    }

    /// <summary>
    /// 异步搜索所有已安装插件和图标包的更新。
    /// 检查完成后，如果有可用更新，会发送系统通知并提供"打开扩展管理器"和"全部更新"按钮。
    /// </summary>
    public static void SearchUpdatesAsync()
    {
        if (_updateCheckRunning)
        {
            return;
        }

        _updateCheckRunning = true;
        PluginManager.PluginsUpdateAvailable.Clear();
        IconManager.IconPacksUpdateAvailable.Clear();
        Task.Run(async () =>
        {
            // 检查所有已启用插件的更新
            foreach (var plugin in PluginManager.Plugins.Values)
            {
                await PluginManager.SearchUpdate(plugin);
            }

            // 检查所有未加载插件的更新
            foreach (var plugin in PluginManager.PluginsNotLoaded.Values)
            {
                await PluginManager.SearchUpdate(plugin);
            }

            // 检查所有扩展商店管理的图标包的更新
            foreach (var iconPack in IconManager.IconPacks.FindAll(iP => iP.ExtensionStoreManaged && !iP.Hidden))
            {
                await IconManager.SearchUpdate(iconPack);
            }

            // 如果有可用更新且尚未发送通知，则发送系统通知
            if (NotificationManager.GetNotification(_updateNotification) == null &&
                PluginManager.PluginsUpdateAvailable.Count + IconManager.IconPacksUpdateAvailable.Count > 0)
            {
                var btnOpenExtensionManager = new ButtonPrimary
                {
                    AutoSize = true,
                    Text = LanguageManager.Strings.OpenExtensionManager
                };
                btnOpenExtensionManager.Click += (sender, e) =>
                {
                    MacroDeck.MainWindow?.SetView(new ExtensionsView());
                };
                var btnUpdateAll = new ButtonPrimary
                {
                    AutoSize = true,
                    Text = LanguageManager.Strings.UpdateAll
                };
                btnUpdateAll.Click += (sender, e) =>
                {
                    NotificationManager.RemoveNotification(_updateNotification);
                    UpdateAllPackages();
                };
                _updateNotification = NotificationManager.SystemNotification("Extension Store",
                    LanguageManager.Strings.UpdatesAvailable,
                    true,
                    icon: Resources.Macro_Deck_2021_update,
                    controls: new List<Control> { btnOpenExtensionManager, btnUpdateAll });
            }

            _updateCheckRunning = false;

            OnUpdateCheckFinished?.Invoke(null, EventArgs.Empty);
        });
    }

    /// <summary>
    /// 批量更新所有有可用更新的插件和图标包。
    /// 跳过已标记为已更新的插件，收集所有待更新的包后统一安装。
    /// </summary>
    public static void UpdateAllPackages()
    {
        var packages = new List<ExtensionStoreDownloaderPackageInfoModel>();

        // 收集所有待更新的插件
        foreach (var updatePlugin in PluginManager.PluginsUpdateAvailable)
        {
            if (PluginManager.UpdatedPlugins.Contains(updatePlugin))
            {
                continue;
            }

            packages.Add(new ExtensionStoreDownloaderPackageInfoModel
            {
                ExtensionType = ExtensionType.Plugin,
                PackageId = GetPackageId(updatePlugin)
            });
        }

        // 收集所有待更新的图标包
        foreach (var updateIconPack in IconManager.IconPacksUpdateAvailable)
        {
            packages.Add(new ExtensionStoreDownloaderPackageInfoModel
            {
                ExtensionType = ExtensionType.IconPack,
                PackageId = updateIconPack.PackageId
            });
        }

        InstallPackages(packages);
    }

    /// <summary>
    /// 检查指定包是否有可用更新。
    /// 通过扩展商店 API V2 查询最新版本，与当前安装版本比较。
    /// </summary>
    /// <param name="packageId">扩展包标识</param>
    /// <param name="installedVersion">当前安装的版本号</param>
    /// <returns>有可用更新返回 true，否则返回 false</returns>
    public static async Task<bool> CheckForAvailableUpdate(string packageId, string installedVersion)
    {
        try
        {
            using var httpClient = new HttpClient();
            var latestFile = await httpClient.GetFromJsonAsync<ApiV2ExtensionFile>(
                $"{Constants.ExtensionStoreApiBaseUrl}/rest/v2/files" +
                $"/{packageId}" +
                $"?apiVersion={MacroDeck.PluginApiVersion}" +
                $"&macroDeckVersion={MacroDeck.Version}");

            if (latestFile?.Version is null || latestFile.Version.Equals(installedVersion))
            {
                return false;
            }

            Logger.Information("Update available for {PackageId}", packageId);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to check for updates for {PackageId}", packageId);
        }

        return false;
    }
}

/// <summary>
/// 扩展类型枚举，区分插件和图标包
/// </summary>
public enum ExtensionType
{
    /// <summary>插件</summary>
    Plugin,

    /// <summary>图标包</summary>
    IconPack
}
