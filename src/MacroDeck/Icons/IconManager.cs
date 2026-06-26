using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Newtonsoft.Json;
using Serilog;
using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.Models;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Icons;

/// <summary>
/// 图标管理器，负责图标包的加载、保存、安装、导出和删除等操作。
/// 管理所有已安装的图标包，并提供图标的查找和添加功能。
/// </summary>
public class IconManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(IconManager));

    /// <summary>已安装的图标包列表</summary>
    public static List<IconPack> IconPacks = new();

    /// <summary>有可用更新的图标包列表</summary>
    public static List<IconPack> IconPacksUpdateAvailable = new();

    /// <summary>图标包安装完成事件</summary>
    public static event EventHandler InstallationFinished;

    /// <summary>图标包变更事件（添加/删除时触发）</summary>
    public static event EventHandler OnIconPacksChanged;

    /// <summary>更新检查完成回调</summary>
    public static Action<object, EventArgs> OnUpdateCheckFinished { get; internal set; }

    /// <summary>图标包加载完成回调</summary>
    public static Action<object, EventArgs> IconPacksLoaded { get; set; }


    /// <summary>
    /// 初始化图标管理器。创建图标包目录并加载所有图标包。
    /// 如果没有图标包，则创建默认的 "My Icons" 图标包。
    /// </summary>
    public static void Initialize()
    {
        if (!Directory.Exists(ApplicationPaths.IconPackDirectoryPath))
        {
            Directory.CreateDirectory(ApplicationPaths.IconPackDirectoryPath);
        }

        LoadIconPacks();
    }

    /// <summary>
    /// 从磁盘加载所有图标包。扫描图标包目录下的每个子目录。
    /// 如果没有任何图标包，则创建默认图标包。
    /// </summary>
    private static void LoadIconPacks()
    {
        IconPacks.Clear();
        Logger.Information("Loading icon packs...");
        foreach (var iconPackDir in Directory.GetDirectories(ApplicationPaths.IconPackDirectoryPath))
        {
            LoadIconPack(iconPackDir);
        }

        if (IconPacks.Count == 0)
        {
            CreateIconPack("My Icons", Environment.UserName, "1.0.0");
        }

        Logger.Information("Loaded {IconPackCount} icon packs", IconPacks.Count);
    }

    /// <summary>
    /// 从指定路径加载单个图标包。
    /// 读取 ExtensionManifest.json 获取元数据，然后加载目录下的所有图片文件。
    /// </summary>
    /// <param name="path">图标包目录路径</param>
    /// <returns>加载成功返回 true，否则返回 false</returns>
    public static bool LoadIconPack(string path)
    {
        var extensionManifestFilePath = Path.Combine(path, "ExtensionManifest.json");
        var extensionIconPath = Path.Combine(path, "ExtensionIcon.png");
        if (!File.Exists(extensionManifestFilePath))
        {
            return false;
        }

        var extensionManifest = ExtensionManifestModel.FromManifestFile(extensionManifestFilePath);
        if (extensionManifest == null)
        {
            return false;
        }

        var iconPack = new IconPack
        {
            Name = extensionManifest.Name,
            Author = extensionManifest.Author,
            Version = extensionManifest.Version,
            PackageId = extensionManifest.PackageId,
            // 通过 .extensionstore 标记文件判断是否由扩展商店管理
            ExtensionStoreManaged = File.Exists(Path.Combine(path, ".extensionstore")),
            // 通过 .hidden 标记文件判断是否隐藏
            Hidden = File.Exists(Path.Combine(path, ".hidden")),
            Icons = new List<Icon>()
        };

        // 如果已存在同名图标包则替换
        if (IconPacks.Find(x => x.PackageId.Equals(iconPack.PackageId)) != null)
        {
            IconPacks.RemoveAll(x => x.PackageId.Equals(iconPack.PackageId));
        }

        IconPacks.Add(iconPack);

        // 加载目录下所有图片文件作为图标（排除 ExtensionIcon）
        foreach (var imageFile in Directory.GetFiles(path).Where(s =>
            s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
            s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                var icon = new Icon
                {
                    FilePath = imageFile,
                    IconId = Path.GetFileNameWithoutExtension(imageFile)
                };
                if (icon.IconId.Equals("ExtensionIcon"))
                {
                    continue;
                }

                iconPack.Icons.Add(icon);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Failed to load icon");
            }
        }

        // 预览图不在此处生成，避免每个图标包长期占用内存
        return true;
    }

    /// <summary>
    /// 异步检查图标包是否有可用更新
    /// </summary>
    /// <param name="iconPack">要检查的图标包</param>
    internal static async Task SearchUpdate(IconPack iconPack)
    {
        var updateAvailable = await ExtensionStoreHelper.CheckForAvailableUpdate(iconPack.PackageId, iconPack.Version);
        if (updateAvailable)
        {
            IconPacksUpdateAvailable.Add(iconPack);
        }
    }

    /// <summary>
    /// 根据名称获取图标包
    /// </summary>
    /// <param name="name">图标包名称</param>
    /// <returns>图标包实例，未找到返回 null</returns>
    public static IconPack? GetIconPackByName(string name)
    {
        return IconPacks.FirstOrDefault(iconPack => iconPack.Name == name);
    }

    /// <summary>
    /// 从指定图标包中获取指定 ID 的图标
    /// </summary>
    /// <param name="iconPack">图标包</param>
    /// <param name="iconId">图标 ID</param>
    /// <returns>图标实例，未找到返回 null</returns>
    public static Icon? GetIcon(IconPack iconPack, string iconId)
    {
        return iconPack?.Icons.FirstOrDefault(icon => icon.IconId == iconId);
    }

    /// <summary>
    /// 根据字符串格式 "图标包名.图标ID" 获取图标
    /// </summary>
    /// <param name="s">格式为 "图标包名.图标ID" 的字符串</param>
    /// <returns>图标实例，未找到返回 null</returns>
    public static Icon? GetIconByString(string s)
    {
        var iconPack = GetIconPackByName(s.Substring(0, s.IndexOf(".")));
        if (iconPack == null)
        {
            return null;
        }

        var icon = GetIcon(iconPack, s.Substring(s.IndexOf(".") + 1));
        return icon;
    }

    /// <summary>
    /// 将图像添加到图标包中。GIF 格式直接保存，其他格式转换为 PNG 保存。
    /// </summary>
    /// <param name="iconPack">目标图标包</param>
    /// <param name="image">要添加的图像</param>
    /// <param name="iconId">图标 ID（为空时自动生成 GUID）</param>
    /// <returns>新添加的图标，失败返回 null</returns>
    public static Icon AddIconImage(IconPack iconPack, Image image, string iconId = "")
    {
        if (iconPack == null || image == null || iconPack.Icons.Find(x => x.IconId == iconId) != null)
        {
            return null;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(iconId))
            {
                iconId = Guid.NewGuid().ToString();
            }

            var format = image.RawFormat;
            var filePath = "";
            if (format.ToString().Equals("Gif", StringComparison.OrdinalIgnoreCase))
            {
                filePath = Path.Combine(ApplicationPaths.IconPackDirectoryPath, iconPack.PackageId, $"{iconId}.gif");
            }
            else
            {
                // 非 GIF 格式创建新位图避免 GDI+ 错误
                filePath = Path.Combine(ApplicationPaths.IconPackDirectoryPath, iconPack.PackageId, $"{iconId}.png");
                image = new Bitmap(image);
                format = ImageFormat.Png;
            }

            image.Save(filePath, format);

            var icon = new Icon
            {
                FilePath = filePath,
                IconId = iconId
            };

            iconPack.Icons.Add(icon);

            return icon;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to add icon to icon pack");
        }

        return null;
    }

    /// <summary>
    /// 导出图标包为 ZIP 文件。先生成预览图，然后将所有文件打包。
    /// </summary>
    /// <param name="iconPack">要导出的图标包</param>
    /// <param name="destination">目标子目录名</param>
    public static void ExportIconPack(IconPack iconPack, string destination)
    {
        var iconPackDir = Path.Combine(ApplicationPaths.IconPackDirectoryPath, iconPack.PackageId);
        try
        {
            // 生成并保存预览图
            using (var preview = iconPack.IconPackIcon ?? IconPackPreview.GeneratePreviewImage(iconPack))
            {
                preview?.Save(Path.Combine(iconPackDir, "ExtensionIcon.png"));
            }

            // 将图标包目录下的所有文件打包为 ZIP
            using var archive = ZipFile.Open(Path.Combine(ApplicationPaths.BackupsDirectoryPath,
                    destination,
                    $"{iconPack.Name}.macroDeckIconPack"),
                ZipArchiveMode.Create);
            if (!Directory.Exists(iconPackDir))
            {
                return;
            }

            foreach (var iconPackFile in new DirectoryInfo(iconPackDir).GetFiles())
            {
                archive.CreateEntryFromFile(Path.Combine(iconPackDir, iconPackFile.Name), iconPackFile.Name);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while exporting icon pack");
        }
    }

    /// <summary>
    /// 删除图标包。从列表中移除并删除磁盘上的目录。
    /// </summary>
    /// <param name="iconPack">要删除的图标包</param>
    public static void DeleteIconPack(IconPack iconPack)
    {
        if (iconPack == null)
        {
            return;
        }

        if (IconPacks.Contains(iconPack))
        {
            IconPacks.Remove(iconPack);
        }

        OnIconPacksChanged?.Invoke(null, EventArgs.Empty);
        try
        {
            Directory.Delete(Path.Combine(ApplicationPaths.IconPackDirectoryPath, iconPack.PackageId), true);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to delete icon pack");
        }
    }

    /// <summary>
    /// 从图标包中删除指定图标。移除图标引用并删除磁盘上的文件。
    /// </summary>
    /// <param name="iconPack">图标包</param>
    /// <param name="icon">要删除的图标</param>
    public static void DeleteIcon(IconPack iconPack, Icon icon)
    {
        if (iconPack == null || icon == null)
        {
            return;
        }

        if (iconPack.Icons.Contains(icon))
        {
            iconPack.Icons.Remove(icon);
        }

        try
        {
            File.Delete(icon.FilePath);
        }
        catch
        {
        }
    }

    /// <summary>
    /// 保存图标包的扩展清单（ExtensionManifest.json）到磁盘。
    /// </summary>
    /// <param name="iconPack">要保存的图标包</param>
    public static void SaveIconPack(IconPack iconPack)
    {
        var extensionManifestModel = new ExtensionManifestModel
        {
            Type = ExtensionType.IconPack,
            Name = iconPack.Name,
            Author = iconPack.Author,
            PackageId = iconPack.PackageId,
            TargetPluginAPIVersion = MacroDeck.PluginApiVersion,
            Version = iconPack.Version
        };

        var iconPackPath = Path.Combine(ApplicationPaths.IconPackDirectoryPath, iconPack.PackageId);

        if (!Directory.Exists(iconPackPath))
        {
            Directory.CreateDirectory(iconPackPath);
        }

        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        try
        {
            using var sw = new StreamWriter(Path.Combine(iconPackPath, "ExtensionManifest.json"));
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, extensionManifestModel);

            Logger.Information("ExtensionManifest saved");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to save ExtensionManifest");
        }
    }

    /// <summary>
    /// 创建新的图标包。自动生成 PackageId（格式：作者名.图标包名），
    /// 保存清单文件并添加到图标包列表。
    /// </summary>
    /// <param name="iconPackName">图标包名称</param>
    /// <param name="author">作者名</param>
    /// <param name="version">版本号</param>
    public static void CreateIconPack(string iconPackName, string author, string version)
    {
        var iconPack = new IconPack
        {
            Name = iconPackName,
            Author = author,
            Version = version,
            PackageId = $"{author.Replace(" ", "").Replace(".", "")}.{iconPackName.Replace(" ", "").Replace(".", "")}",
            Icons = new List<Icon>()
        };

        SaveIconPack(iconPack);

        IconPacks.Add(iconPack);
    }

    /// <summary>
    /// 从 ZIP 文件安装图标包。
    /// 验证清单文件和类型，解压到图标包目录，支持扩展商店管理的标记。
    /// </summary>
    /// <param name="location">ZIP 文件路径</param>
    /// <param name="extensionStoreManaged">是否由扩展商店管理</param>
    /// <returns>安装成功的图标包，失败返回 null</returns>
    public static IconPack InstallIconPackZip(string location, bool extensionStoreManaged = false)
    {
        try
        {
            var extensionManifestModel = ExtensionManifestModel.FromZipFilePath(location);
            if (extensionManifestModel == null)
            {
                Logger.Error("{Location} does not contain a manifest file!", location);
                return null;
            }

            if (extensionManifestModel.Type != ExtensionType.IconPack)
            {
                Logger.Error("{PackageId} is not a icon pack!", extensionManifestModel.PackageId);
                return null;
            }

            var destinationPath
                = Path.Combine(ApplicationPaths.IconPackDirectoryPath, extensionManifestModel.PackageId);
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
                // 扩展商店管理时，记录下载统计
                if (extensionStoreManaged)
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.GetStringAsync(
                            $"https://macrodeck.org/extensionstore/extensionstore.php?action=count-download&package-id={extensionManifestModel.PackageId}").GetAwaiter().GetResult();
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                // 目录已存在则先删除（覆盖安装）
                Directory.Delete(destinationPath, true);
            }

            ZipFile.ExtractToDirectory(location, destinationPath);
            // 创建扩展商店管理标记文件
            if (extensionStoreManaged)
            {
                try
                {
                    File.Create(Path.Combine(destinationPath, ".extensionstore"));
                }
                catch
                {
                }
            }

            if (LoadIconPack(destinationPath))
            {
                // 从可用更新列表中移除
                if (IconPacksUpdateAvailable.Find(x => x.PackageId.Equals(extensionManifestModel.PackageId)) != null)
                {
                    IconPacksUpdateAvailable.Remove(IconPacksUpdateAvailable.Find(x =>
                        x.PackageId.Equals(extensionManifestModel.PackageId)));
                }

                Logger.Information("Successfully installed {PackageId}", extensionManifestModel.PackageId);
                var iconPack = GetIconPackByName(extensionManifestModel.Name);
                InstallationFinished?.Invoke(iconPack, EventArgs.Empty);
                return iconPack;
            }

            Logger.Error("{PackageId} is maybe corruped", extensionManifestModel.PackageId);
            return null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while installing icon pack from zip");
        }

        return null;
    }
}
