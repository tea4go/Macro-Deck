using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.InternalPlugins.ActionButtonPlugin;
using SuchByte.MacroDeck.InternalPlugins.DevicePlugin;
using Serilog;
using SuchByte.MacroDeck.Folders.Plugin;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Models;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables.Plugin;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// 插件管理器，负责插件的加载、安装、更新和删除等全生命周期管理。
/// 管理所有已加载的插件、受保护的内部插件、未加载的插件和可用更新的插件。
/// </summary>
public static class PluginManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(PluginManager));

    /// <summary>插件变更事件</summary>
    public static event EventHandler? OnPluginsChange;


    /// <summary>已加载的插件字典，键为插件名称</summary>
    public static Dictionary<string, MacroDeckPlugin> Plugins { get; } = new();

    /// <summary>受保护的插件列表（内部插件，不可被用户卸载）</summary>
    public static List<MacroDeckPlugin> ProtectedPlugins { get; } = new();

    /// <summary>已更新待重启的插件列表</summary>
    public static List<MacroDeckPlugin> UpdatedPlugins { get; } = new();

    /// <summary>未能成功加载的插件字典，键为插件名称</summary>
    public static Dictionary<string, MacroDeckPlugin> PluginsNotLoaded { get; } = new();

    /// <summary>有可用更新的插件列表</summary>
    public static List<MacroDeckPlugin> PluginsUpdateAvailable { get; } = new();

    /// <summary>插件目录映射，键为插件实例，值为插件所在目录路径</summary>
    public static Dictionary<MacroDeckPlugin, string> PluginDirectories { get; } = new();

    private const string ManifestFileName = "ExtensionManifest.json";
    private const string DeleteMarkerFileName = ".delete";

    private static bool _loaded;

    /// <summary>
    /// 加载所有插件。首先处理待更新的插件（从 .updates 目录复制到插件目录），
    /// 然后扫描插件目录加载每个插件，最后添加内部插件。
    /// 仅在首次调用时执行，后续调用直接返回。
    /// </summary>
    public static void Load()
    {
        if (_loaded)
        {
            return;
        }

        Logger.Information("Loading plugins...");
        _loaded = true;
        Plugins.Clear();
        PluginsUpdateAvailable.Clear();
        PluginsNotLoaded.Clear();
        ProtectedPlugins.Clear();
        PluginDirectories.Clear();
        if (!Directory.Exists(ApplicationPaths.PluginsDirectoryPath))
        {
            Directory.CreateDirectory(ApplicationPaths.PluginsDirectoryPath);
        }

        // 处理待更新的插件：从 .updates 目录复制到插件目录
        if (Directory.Exists(ApplicationPaths.UpdatePluginsDirectoryPath))
        {
            foreach (var directory in Directory.GetDirectories(ApplicationPaths.UpdatePluginsDirectoryPath))
            {
                try
                {
                    var destinationDirectory = Path.Combine(ApplicationPaths.PluginsDirectoryPath,
                        new DirectoryInfo(directory).Name);
                    DirectoryCopy.Copy(directory, destinationDirectory, true);
                    Directory.Delete(directory, true);
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                Directory.Delete(ApplicationPaths.UpdatePluginsDirectoryPath, true);
            }
            catch
            {
                // ignored
            }
        }

        // 加载插件目录中的所有插件
        foreach (var directory in Directory.GetDirectories(ApplicationPaths.PluginsDirectoryPath))
        {
            // 如果存在 .delete 标记文件，删除插件目录
            if (File.Exists(Path.Combine(directory, DeleteMarkerFileName)))
            {
                try
                {
                    File.Delete(Path.Combine(directory, DeleteMarkerFileName));
                    Directory.Delete(directory, true);
                }
                catch
                {
                    // ignored
                }

                continue;
            }

            var manifestFile = Path.Combine(directory, ManifestFileName);
            if (!File.Exists(manifestFile))
            {
                continue;
            }

            try
            {
                var extensionManifest = ExtensionManifestModel.FromManifestFile(manifestFile);
                if (extensionManifest == null)
                {
                    continue;
                }

                var plugin = LoadPlugin(extensionManifest, directory);
                plugin.Author = extensionManifest.Author;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while deserializing manifest for {Directory}", directory);
            }
        }

        // 添加内部插件（受保护，不可卸载）
        AddAndEnablePlugin(new ActionButtonPlugin(), true);
        AddAndEnablePlugin(new VariablesPlugin(), true);
        AddAndEnablePlugin(new FolderPlugin(), true);
        AddAndEnablePlugin(new DevicePlugin(), true);
    }

    /// <summary>
    /// 添加并启用插件
    /// </summary>
    /// <param name="macroDeckPlugin">插件实例</param>
    /// <param name="protect">是否受保护（不可卸载）</param>
    private static void AddAndEnablePlugin(MacroDeckPlugin macroDeckPlugin, bool protect = false)
    {
        AddPlugin(macroDeckPlugin, protect);
        macroDeckPlugin.Enable();
    }

    /// <summary>
    /// 从扩展清单和目录加载单个插件。
    /// 通过反射加载插件程序集，查找 MacroDeckPlugin 子类并实例化。
    /// 加载成功后异步检查更新并启用插件。
    /// </summary>
    /// <param name="extensionManifest">扩展清单</param>
    /// <param name="pluginDirectory">插件目录路径</param>
    /// <returns>加载的插件实例，失败返回 null 或 DisabledPlugin</returns>
    private static MacroDeckPlugin? LoadPlugin(ExtensionManifestModel extensionManifest, string pluginDirectory)
    {
        Assembly? asm = null;
        try
        {
            asm = Assembly.LoadFrom(Path.Combine(pluginDirectory, extensionManifest.Dll));
            Logger.Information("Loading plugin {PluginName}", asm.GetName().Name);

            foreach (var type in asm.GetTypes())
            {
                try
                {
                    // 查找 MacroDeckPlugin 的子类并实例化
                    if (!type.IsClass ||
                        !type.IsSubclassOf(typeof(MacroDeckPlugin)) ||
                        Activator.CreateInstance(type) is not MacroDeckPlugin plugin)
                    {
                        continue;
                    }

                    AddPlugin(plugin);
                    PluginDirectories[plugin] = pluginDirectory;
                    // 异步检查更新
                    Task.Run(async () =>
                        await SearchUpdate(plugin));
                    // 加载插件图标
                    if (File.Exists(Path.Combine(pluginDirectory, "ExtensionIcon.png")))
                    {
                        plugin.PluginIcon =
                            (Image)Image.FromFile(Path.Combine(pluginDirectory, "ExtensionIcon.png")).Clone();
                    }

                    // 异步启用插件
                    Task.Run(plugin.Enable);
                    return plugin;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error while loading plugin");
                }
            }
        }
        catch (Exception ex)
        {
            // 加载失败时创建 DisabledPlugin 并进入安全模式
            if (asm != null)
            {
                var disabledPlugin = new DisabledPlugin
                {
                    Name = asm.GetName().Name,
                    Version = FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion,
                    Author = extensionManifest.Author
                };

                PluginsNotLoaded[disabledPlugin.Name] = disabledPlugin;
                PluginDirectories[disabledPlugin] = pluginDirectory;

                MacroDeck.SafeMode = true;
                Logger.Warning("Cannot load {PluginName} version {PluginVersion}. Macro Deck was started in safe mode.",
                    disabledPlugin.Name,
                    disabledPlugin.Version);
                return disabledPlugin;
            }
        }

        return null;
    }

    /// <summary>
    /// 异步检查插件是否有可用更新
    /// </summary>
    /// <param name="plugin">要检查的插件</param>
    internal static async Task SearchUpdate(MacroDeckPlugin plugin)
    {
        if (UpdatedPlugins.Contains(plugin) || ProtectedPlugins.Contains(plugin))
        {
            return;
        }

        var updateAvailable =
            await ExtensionStoreHelper.CheckForAvailableUpdate(ExtensionStoreHelper.GetPackageId(plugin),
                plugin.Version);
        if (updateAvailable)
        {
            PluginsUpdateAvailable.Add(plugin);
        }
    }

    /// <summary>
    /// 检查指定名称的插件是否已安装
    /// </summary>
    /// <param name="name">插件名称</param>
    /// <returns>已安装返回 true</returns>
    public static bool IsInstalled(string name)
    {
        return Plugins.Any(x => x.Key.Replace(" ", string.Empty).Trim().Equals(name.Replace(" ", string.Empty).Trim(),
            StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// 删除指定插件。通过创建 .delete 标记文件实现延迟删除，
    /// 插件在下次启动时被实际删除。受保护的插件不可删除。
    /// </summary>
    /// <param name="name">插件名称</param>
    public static void DeletePlugin(string name)
    {
        if (!Plugins.ContainsKey(name))
        {
            return;
        }

        MacroDeckPlugin plugin = null;
        if (Plugins.ContainsKey(name))
        {
            plugin = Plugins[name];
        }
        else if (PluginsNotLoaded.ContainsKey(name))
        {
            plugin = PluginsNotLoaded[name];
        }

        if (plugin == null)
        {
            return;
        }

        // 受保护的插件不可删除
        if (ProtectedPlugins.Contains(plugin))
        {
            return;
        }

        if (PluginDirectories.ContainsKey(plugin))
        {
            try
            {
                if (UpdatedPlugins.Contains(plugin))
                {
                    UpdatedPlugins.Remove(plugin);
                }

                if (PluginsNotLoaded.ContainsKey(name))
                {
                    PluginsNotLoaded.Remove(name);
                }

                if (PluginsUpdateAvailable.Contains(plugin))
                {
                    PluginsUpdateAvailable.Remove(plugin);
                }

                if (Plugins.ContainsKey(name))
                {
                    Plugins.Remove(name);
                }

                // 创建删除标记文件，下次启动时删除
                var deleteMarkerFile = Path.Combine(PluginDirectories[plugin], DeleteMarkerFileName);
                File.Create(deleteMarkerFile);
                Logger.Information("{PluginName} deleted", name);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        OnPluginsChange?.Invoke(name, EventArgs.Empty);
    }

    /// <summary>
    /// 从 ZIP 文件安装插件。先解压到临时目录，再调用安装方法。
    /// </summary>
    /// <param name="zipFilePath">ZIP 文件路径</param>
    internal static void InstallPluginFromZip(string zipFilePath)
    {
        var extensionManifest = ExtensionManifestModel.FromZipFilePath(zipFilePath);
        var extractedDirectory = Path.Combine(ApplicationPaths.TempDirectoryPath, extensionManifest.PackageId);
        ZipFile.ExtractToDirectory(Path.Combine(ApplicationPaths.TempDirectoryPath, zipFilePath),
            extractedDirectory,
            true);

        InstallPlugin(extractedDirectory, extensionManifest.PackageId);
    }

    /// <summary>
    /// 安装或更新插件。如果是更新，将文件复制到 .updates 目录待下次启动应用；
    /// 如果是新安装，直接加载插件并可选打开配置器。
    /// </summary>
    /// <param name="directory">插件源目录</param>
    /// <param name="packageName">插件包名</param>
    internal static void InstallPlugin(string directory, string packageName)
    {
        var update = Directory.Exists(Path.Combine(ApplicationPaths.PluginsDirectoryPath, packageName));
        Logger.Information("{InstallAction} {PackageName}", update ? "Updating" : "Installing", packageName);
        Assembly asm = null;
        var error = false;
        var extensionManifest = new ExtensionManifestModel();
        try
        {
            var installationDirectory = Path.Combine(ApplicationPaths.PluginsDirectoryPath, packageName);
            if (update)
            {
                // 更新时复制到 .updates 目录，下次启动时应用
                installationDirectory = Path.Combine(ApplicationPaths.UpdatePluginsDirectoryPath, packageName);
            }

            DirectoryCopy.Copy(directory, installationDirectory, true);

            if (!update)
            {
                // 新安装：加载清单和插件
                var manifestFile = Path.Combine(installationDirectory, ManifestFileName);
                if (!File.Exists(manifestFile))
                {
                    error = true;
                }
                else
                {
                    try
                    {
                        extensionManifest = ExtensionManifestModel.FromManifestFile(manifestFile);
                        if (extensionManifest == null)
                        {
                            error = true;
                        }
                        else
                        {
                            var plugin = LoadPlugin(extensionManifest, installationDirectory);
                            plugin.Author = extensionManifest.Author;

                            // 如果插件需要配置，提示用户
                            try
                            {
                                if (plugin != null && plugin.CanConfigure)
                                {
                                    using var msgBox = new MessageBox();
                                    if (msgBox.ShowDialog(LanguageManager.Strings.PluginNeedsConfiguration,
                                            string.Format(LanguageManager.Strings.ConfigureNow, plugin.Name),
                                            MessageBoxButtons.YesNo) ==
                                        DialogResult.Yes)
                                    {
                                        plugin.OpenConfigurator();
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        error = true;
                    }
                }
            }
            else
            {
                // 更新：记录到已更新列表
                UpdatedPlugins.Add(PluginDirectories.FirstOrDefault(x =>
                    x.Value == Path.Combine(ApplicationPaths.PluginsDirectoryPath, packageName)).Key);
            }

            OnPluginsChange?.Invoke(null, EventArgs.Empty);
        }
        catch
        {
            error = true;
        }

        if (error)
        {
            if (asm != null && extensionManifest != null)
            {
                var disabledPlugin = new DisabledPlugin
                {
                    Name = asm.GetName().Name,
                    Version = asm.GetName().Version.ToString(),
                    Author = extensionManifest.Author
                };

                PluginsNotLoaded[asm.GetName().Name] = disabledPlugin;

                using var msgBox = new MessageBox();
                msgBox.ShowDialog(LanguageManager.Strings.ErrorWhileInstallingPlugin,
                    string.Format(LanguageManager.Strings.PluginXCouldNotBeInstalled, asm.GetName().Name),
                    MessageBoxButtons.OK);
            }
        }
    }

    /// <summary>
    /// 将插件添加到插件列表
    /// </summary>
    /// <param name="macroDeckPlugin">插件实例</param>
    /// <param name="protect">true = 插件不可被用户卸载</param>
    private static void AddPlugin(MacroDeckPlugin macroDeckPlugin, bool protect = false)
    {
        if (!Plugins.ContainsKey(macroDeckPlugin.Name))
        {
            Plugins[macroDeckPlugin.Name] = macroDeckPlugin;
            if (protect)
            {
                ProtectedPlugins.Add(macroDeckPlugin);
            }
        }

        OnPluginsChange?.Invoke(macroDeckPlugin, EventArgs.Empty);
    }

    /// <summary>
    /// 根据名称从插件中获取动作的新实例（通过 XML 序列化/反序列化复制）
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <param name="name">动作名称</param>
    /// <returns>动作的新实例，未找到返回 null</returns>
    public static PluginAction GetActionByName(MacroDeckPlugin plugin, string name)
    {
        var action = plugin.Actions.Find(plugin => plugin.Name.Equals(name));
        if (action == null)
        {
            return null;
        }

        using var ms = new MemoryStream();
        var serializer = new XmlSerializer(action.GetType());
        serializer.Serialize(ms, action);
        ms.Seek(0, SeekOrigin.Begin);
        return (PluginAction)serializer.Deserialize(ms);
    }

    /// <summary>
    /// 通过 XML 序列化/反序列化创建插件动作的新实例
    /// </summary>
    /// <param name="action">要复制的动作</param>
    /// <returns>新的动作实例，输入为 null 时返回 null</returns>
    public static PluginAction GetNewActionInstance(PluginAction action)
    {
        if (action == null)
        {
            return null;
        }

        using var ms = new MemoryStream();
        var serializer = new XmlSerializer(action.GetType());
        serializer.Serialize(ms, action);
        ms.Seek(0, SeekOrigin.Begin);
        return (PluginAction)serializer.Deserialize(ms);
    }

    /// <summary>
    /// 根据动作实例查找所属插件
    /// </summary>
    /// <param name="pluginAction">插件动作</param>
    /// <returns>所属插件，未找到返回 DisabledPlugin</returns>
    public static MacroDeckPlugin GetPluginByAction(PluginAction? pluginAction)
    {
        foreach (var macroDeckPlugin in Plugins.Values)
        {
            if (macroDeckPlugin.Actions.Find(mdp => mdp.GetType().FullName.Equals(pluginAction.GetType().FullName)) !=
                null)
            {
                return macroDeckPlugin;
            }
        }

        return new DisabledPlugin
        {
            Name = "Plugin not available"
        };
    }

    /// <summary>
    /// 根据名称获取插件
    /// </summary>
    /// <param name="name">插件名称</param>
    /// <returns>插件实例，未找到返回 DisabledPlugin</returns>
    public static MacroDeckPlugin GetPluginByName(string name)
    {
        foreach (var macroDeckPlugin in Plugins.Values)
        {
            if (macroDeckPlugin.Name.Equals(name))
            {
                return macroDeckPlugin;
            }
        }

        return new DisabledPlugin
        {
            Name = "Plugin not available"
        };
    }
}
