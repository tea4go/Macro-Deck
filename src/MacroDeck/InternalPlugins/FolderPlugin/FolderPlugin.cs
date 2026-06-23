using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.InternalPlugins.FolderPlugin.GUI;
using Serilog;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;
using SuchByte.MacroDeck.Server;

// ReSharper disable once CheckNamespace
namespace SuchByte.MacroDeck.Folders.Plugin;
// Don't change because of backwards compatibility!

/// <summary>
/// 文件夹内部插件，提供文件夹切换相关动作。
/// 包含切换到指定文件夹、返回上一级文件夹和返回根文件夹等动作。
/// </summary>
public class FolderPlugin : MacroDeckPlugin
{
    /// <summary>插件名称</summary>
    internal override string Name => LanguageManager.Strings.PluginMacroDeckFolder;

    /// <summary>插件作者</summary>
    internal override string Author => "Macro Deck";

    /// <summary>
    /// 启用插件，注册文件夹切换相关动作
    /// </summary>
    public override void Enable()
    {
        Actions = new List<PluginAction>
        {
            new FolderSwitcher(),
            new GoToParentFolder(),
            new GoToRootFolder()
        };
    }
}

/// <summary>
/// 文件夹切换动作，根据配置切换到指定的文件夹。
/// 支持本地 Macro Deck 软件和已连接的远程设备两种触发来源。
/// </summary>
public class FolderSwitcher : PluginAction
{
    private static readonly ILogger Logger = Log.ForContext(typeof(FolderSwitcher));

    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionChangeFolder;

    /// <summary>是否可配置</summary>
    public override bool CanConfigure => true;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionChangeFolderDescription;

    /// <summary>
    /// 触发文件夹切换动作。
    /// 根据 clientId 区分触发来源：
    /// - 空字符串或 "-1"：Macro Deck 软件本身，切换主窗口的当前文件夹
    /// - 其他值：已连接的远程设备，切换该设备的当前文件夹
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        Logger.Debug("Switch folder triggered by {ClientId}", clientId);
        switch (clientId)
        {
            // ClientID -1 or "" = Macro Deck software itself
            case "":
            case "-1":
                // 本地软件：切换主窗口 DeckView 的文件夹
                if (MacroDeck.MainWindow != null && MacroDeck.MainWindow.DeckView != null)
                {
                    MacroDeck.MainWindow.DeckView.SetFolder(
                        ProfileManager.FindFolderById(Configuration, ProfileManager.CurrentProfile));
                }

                break;
            // ClientId != -1 = Connected device
            default:
                // 远程设备：切换该客户端的文件夹
                MacroDeckServer.SetFolder(MacroDeckServer.GetMacroDeckClient(clientId),
                    ProfileManager.FindFolderById(Configuration, MacroDeckServer.GetMacroDeckClient(clientId).Profile));
                break;
        }
    }

    /// <summary>
    /// 获取动作配置控件
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>文件夹切换配置器控件</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new FolderSwitcherConfigurator(this);
    }
}

/// <summary>
/// 返回上一级文件夹动作
/// </summary>
public class GoToParentFolder : PluginAction
{
    private static readonly ILogger Logger = Log.ForContext(typeof(GoToParentFolder));

    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionGoToParentFolder;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionGoToParentFolderDescription;

    /// <summary>
    /// 触发返回上一级文件夹动作。
    /// 根据 clientId 区分触发来源（本地软件或远程设备），
    /// 查找当前文件夹的父文件夹并切换。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        try
        {
            Logger.Debug("Go to parent folder triggered by {ClientId}", clientId);
            switch (clientId)
            {
                // ClientID -1 or "" = Macro Deck software itself
                case "":
                case "-1":
                    // 本地软件：切换主窗口 DeckView 到父文件夹
                    if (MacroDeck.MainWindow != null && MacroDeck.MainWindow.DeckView != null)
                    {
                        MacroDeck.MainWindow.DeckView.SetFolder(ProfileManager.FindParentFolder(
                            MacroDeck.MainWindow.DeckView.CurrentFolder,
                            ProfileManager.CurrentProfile));
                    }

                    break;
                // ClientId != -1 = Connected device
                default:
                    // 远程设备：切换该客户端到父文件夹
                    var macroDeckClient = MacroDeckServer.GetMacroDeckClient(clientId);
                    var parentFolder = ProfileManager.FindParentFolder(macroDeckClient.Folder, macroDeckClient.Profile);
                    MacroDeckServer.SetFolder(macroDeckClient, parentFolder);
                    break;
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 获取动作配置控件（此动作不可配置）
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>始终返回 null</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return null;
    }
}

/// <summary>
/// 返回根文件夹动作
/// </summary>
public class GoToRootFolder : PluginAction
{
    private static readonly ILogger Logger = Log.ForContext(typeof(GoToRootFolder));

    /// <summary>动作名称</summary>
    public override string Name => LanguageManager.Strings.ActionGoToRootFolder;

    /// <summary>动作描述</summary>
    public override string Description => LanguageManager.Strings.ActionGoToRootFolderDescription;

    /// <summary>
    /// 触发返回根文件夹动作。
    /// 根据 clientId 区分触发来源（本地软件或远程设备），
    /// 查找配置文件中的根文件夹（IsRootFolder = true）并切换。
    /// </summary>
    /// <param name="clientId">触发客户端 ID</param>
    /// <param name="actionButton">触发源动作按钮</param>
    public override void Trigger(string clientId, ActionButton.ActionButton actionButton)
    {
        try
        {
            Logger.Debug("Go to root folder triggered by {ClientId}", clientId);
            switch (clientId)
            {
                // ClientID -1 or "" = Macro Deck software itself
                case "":
                case "-1":
                    // 本地软件：切换主窗口 DeckView 到根文件夹
                    if (MacroDeck.MainWindow != null && MacroDeck.MainWindow.DeckView != null)
                    {
                        MacroDeck.MainWindow.DeckView.SetFolder(
                            ProfileManager.CurrentProfile.Folders.Find(folder => folder.IsRootFolder));
                    }

                    break;
                // ClientId != -1 = Connected device
                default:
                    // 远程设备：切换该客户端到根文件夹
                    var macroDeckClient = MacroDeckServer.GetMacroDeckClient(clientId);
                    var rootFolder = macroDeckClient.Profile.Folders.Find(folder => folder.IsRootFolder);
                    MacroDeckServer.SetFolder(macroDeckClient, rootFolder);
                    break;
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 获取动作配置控件（此动作不可配置）
    /// </summary>
    /// <param name="actionConfigurator">动作配置器</param>
    /// <returns>始终返回 null</returns>
    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return null;
    }
}
