using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using SQLite;
using SuchByte.MacroDeck.CottleIntegration;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Folders;
using Serilog;
using SuchByte.MacroDeck.JSON;
using SuchByte.MacroDeck.Language;
using SuchByte.MacroDeck.Properties;
using SuchByte.MacroDeck.Server;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;
using SuchByte.MacroDeck.Variables;
using SuchByte.MacroDeck.WindowFocus;

namespace SuchByte.MacroDeck.Profiles;

/// <summary>
/// 配置文件管理器，负责所有配置文件（Profile）的加载、保存、创建、删除，以及
/// 文件夹与按钮的 CRUD 操作。同时监听变量变化以刷新按钮标签，监听窗口焦点变化
/// 以自动切换当前活动文件夹。这是一个静态类，全局唯一。
/// </summary>
public static class ProfileManager
{
    /// <summary>
    /// Serilog 日志记录器实例，用于记录配置文件管理过程中的调试、信息和错误日志。
    /// </summary>
    private static readonly ILogger Logger = Log.ForContext(typeof(ProfileManager));

    /// <summary>
    /// 在所有配置文件保存完成后触发，通知订阅者刷新 UI 或执行后续操作。
    /// </summary>
    public static event EventHandler? ProfilesSaved;
    /// <summary>
    /// 在新配置文件创建完成后触发，通知订阅者（如设备管理器）进行初始化。
    /// </summary>
    public static event EventHandler? ProfileCreated;

    /// <summary>
    /// 获取或设置当前选中的配置文件。设备客户端会根据此配置显示对应的按钮布局。
    /// </summary>
    public static MacroDeckProfile? CurrentProfile { get; set; }

    /// <summary>
    /// 获取当前加载的所有配置文件列表。通过 <see cref="Load"/> 方法填充。
    /// </summary>
    public static List<MacroDeckProfile> Profiles { get; private set; } = [];

    /// <summary>
    /// 用于序列化保存操作的互斥锁，确保同一时间只有一个写操作在进行，
    /// 防止并发写入导致 JSON 文件损坏。
    /// </summary>
    private static readonly Lock SaveLock = new();

    /// <summary>
    /// 历史记录字典，键为客户端，值为（上一个文件夹, 触发此切换的进程名）。
    /// 用于窗口焦点离开应用时，将客户端恢复到之前的文件夹状态。
    /// </summary>
    private static readonly ConcurrentDictionary<MacroDeckClient, (MacroDeckFolder PreviousFolder, string ProcessName)>
        History = new();

    /// <summary>
    /// 注册变量变更监听器。当 <see cref="VariableManager"/> 中的变量值发生变化时，
    /// 自动更新所有引用该变量的按钮标签。
    /// </summary>
    public static void AddVariableChangedListener()
    {
        VariableManager.OnVariableChanged += VariableChanged;
    }

    /// <summary>
    /// 窗口焦点检测器实例，通过轮询当前活动窗口来触发焦点切换逻辑。
    /// </summary>
    private static WindowFocusDetection? _windowFocusDetection;

    /// <summary>
    /// 注册窗口焦点变更监听器。创建 <see cref="WindowFocusDetection"/> 实例并开始
    /// 监听活动窗口切换，同时注册应用程序退出事件以进行资源清理。
    /// </summary>
    public static void AddWindowFocusChangedListener()
    {
        _windowFocusDetection = new WindowFocusDetection();
        _windowFocusDetection.OnWindowFocusChanged += OnWindowFocusChanged;

        Application.ApplicationExit += OnApplicationExit;
    }

    /// <summary>
    /// 应用程序退出时的清理逻辑。取消所有事件订阅，
    /// 释放 <see cref="WindowFocusDetection"/> 实例并置为 null。
    /// </summary>
    /// <param name="sender">事件源（Application 实例）。</param>
    /// <param name="e">事件参数。</param>
    private static void OnApplicationExit(object? sender, EventArgs e)
    {
        Application.ApplicationExit -= OnApplicationExit;
        if (_windowFocusDetection != null)
        {
            _windowFocusDetection.OnWindowFocusChanged -= OnWindowFocusChanged;
            _windowFocusDetection.Dispose();
            _windowFocusDetection = null;
        }
    }

    /// <summary>
    /// 窗口焦点变更事件处理程序。在后台线程中异步调用文件夹切换逻辑，
    /// 避免阻塞 UI 线程。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">包含新旧进程名称的事件参数。</param>
    private static void OnWindowFocusChanged(object sender, WindowChangedEventArgs e)
    {
        // 使用 Task.Run 在后台线程执行，避免延时影响焦点检测的实时性
        _ = Task.Run(() => UpdateSetFolderForProcess(e.NewProcess, e.PreviousProcess));
    }

    /// <summary>
    /// 根据前台窗口进程名称自动切换设备客户端所显示的文件夹。
    /// 当检测到活动窗口切换到新进程时：
    /// 1. 查找离开旧进程的客户端，将其恢复到之前的文件夹；
    /// 2. 查找匹配新进程的文件夹，将对应设备的客户端切换到该文件夹。
    /// </summary>
    /// <param name="newProcess">新激活的窗口进程名称（如 "notepad.exe"）。</param>
    /// <param name="oldProcess">之前的窗口进程名称。</param>
    private static void UpdateSetFolderForProcess(string newProcess, string oldProcess)
    {
        // 空进程名无法匹配任何文件夹，直接返回
        if (string.IsNullOrWhiteSpace(newProcess))
        {
            return;
        }

        foreach (var pair in History
            .Where(x =>
                string.Equals(x.Value.ProcessName, oldProcess, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(x.Key.Folder?.ApplicationToTrigger,
                    newProcess,
                    StringComparison.OrdinalIgnoreCase))
            .ToArray())
        {
            MacroDeckServer.SetFolder(pair.Key, pair.Value.PreviousFolder);
        }

        var affectedProfiles = Profiles
            .Where(profile =>
                profile.Folders.Any(folder =>
                    string.Equals(folder.ApplicationToTrigger,
                        newProcess,
                        StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        foreach (var profile in affectedProfiles)
        {
            foreach (var folder in profile.Folders.Where(folder =>
                string.Equals(folder.ApplicationToTrigger,
                    newProcess,
                    StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var deviceId in folder.ApplicationsFocusDevices)
                {
                    var device = DeviceManager.GetMacroDeckDevice(deviceId);

                    if (device is null ||
                        !device.Available ||
                        string.IsNullOrWhiteSpace(device.ClientId))
                    {
                        continue;
                    }

                    var client = MacroDeckServer.GetMacroDeckClient(device.ClientId);

                    if (client is null ||
                        !ReferenceEquals(client.Profile, profile) ||
                        ReferenceEquals(client.Folder, folder))
                    {
                        continue;
                    }

                    History[client] = (client.Folder, newProcess);
                    MacroDeckServer.SetFolder(client, folder);
                }
            }
        }
    }

    /// <summary>
    /// 变量变更事件处理程序。当 <see cref="VariableManager"/> 中任意变量发生变化时，
    /// 触发所有引用该变量的按钮标签更新。
    /// </summary>
    /// <param name="sender">事件源，应为 <see cref="Variable"/> 实例。</param>
    /// <param name="e">事件参数。</param>
    private static void VariableChanged(object sender, EventArgs e)
    {
        if (sender is Variable variable)
        {
            UpdateAllVariableLabels(variable);
        }
    }

    /// <summary>
    /// 更新所有引用了指定变量的按钮标签。遍历所有配置文件->文件夹->按钮，
    /// 筛选出 LabelOff 或 LabelOn 文本中包含该变量名的按钮，然后并行更新它们的标签图像。
    /// </summary>
    /// <param name="variable">发生变化的变量实例。</param>
    public static void UpdateAllVariableLabels(Variable variable)
    {
        if (string.IsNullOrWhiteSpace(variable?.Name))
        {
            return;
        }

        var variableName = variable.Name;

        var buttons = Profiles
            .SelectMany(profile => profile.Folders)
            .SelectMany(folder => folder.ActionButtons)
            .Where(button =>
                (button.LabelOff?.LabelText.Contains(variableName, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (button.LabelOn?.LabelText.Contains(variableName, StringComparison.OrdinalIgnoreCase) ?? false))
            .Distinct()
            .ToArray();

        Parallel.ForEach(buttons, UpdateVariableLabels);
    }

    /// <summary>
    /// 更新单个按钮的标签图像。使用 Cottle 模板引擎渲染标签文本中的变量引用，
    /// 然后生成对应的 Base64 编码图像，并通过 WebSocket 推送给所有连接的客户端。
    /// </summary>
    /// <param name="actionButton">需要更新标签的按钮实例。</param>
    public static void UpdateVariableLabels(ActionButton.ActionButton actionButton)
    {
        if (actionButton?.LabelOff == null || actionButton.LabelOn == null)
        {
            return;
        }

        try
        {
            var labelOffText = TemplateManager.RenderTemplate(actionButton.LabelOff.LabelText);
            var labelOnText = TemplateManager.RenderTemplate(actionButton.LabelOn.LabelText);

            using (var labelOffBitmap = new Bitmap(250, 250))
            using (var labelOffFont = new Font(actionButton.LabelOff.FontFamily, actionButton.LabelOff.Size))
            using (var labelOffImage = LabelGenerator.GetLabel(labelOffBitmap,
                labelOffText,
                actionButton.LabelOff.LabelPosition,
                labelOffFont,
                actionButton.LabelOff.LabelColor,
                Color.Black,
                new SizeF(2f, 2f)))
            {
                actionButton.LabelOff.LabelBase64 = Base64.GetBase64FromImage(labelOffImage);
            }

            using (var labelOnBitmap = new Bitmap(250, 250))
            using (var labelOnFont = new Font(actionButton.LabelOn.FontFamily, actionButton.LabelOn.Size))
            using (var labelOnImage = LabelGenerator.GetLabel(labelOnBitmap,
                labelOnText,
                actionButton.LabelOn.LabelPosition,
                labelOnFont,
                actionButton.LabelOn.LabelColor,
                Color.Black,
                new SizeF(2f, 2f)))
            {
                actionButton.LabelOn.LabelBase64 = Base64.GetBase64FromImage(labelOnImage);
            }

            foreach (var client in MacroDeckServer.Clients)
            {
                MacroDeckServer.SendButton(client, actionButton);
            }
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// 加载所有配置文件。从 Profiles 目录读取 JSON 文件并反序列化为 <see cref="MacroDeckProfile"/> 对象。
    /// 首次启动时会自动创建默认配置文件。加载完成后会初始化所有按钮的插件动作绑定和标签。
    /// </summary>
    internal static void Load()
    {
        Logger.Information("Loading profiles...");
        Profiles = [];

        MigrateLegacyDatabase();

        var profilesDir = ApplicationPaths.ProfilesDirectoryPath;
        foreach (var file in Directory.GetFiles(profilesDir, "*.json"))
        {
            try
            {
                var jsonString = File.ReadAllText(file);
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore,
                    Error = (sender, args) =>
                    {
                        Logger.Error(args.ErrorContext.Error, "Error while deserializing profile {File}", file);
                        args.ErrorContext.Handled = true;
                    },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var profile = JsonConvert.DeserializeObject<MacroDeckProfile>(jsonString, settings);
                if (profile is null)
                {
                    continue;
                }

                if (profile.ProfileId == "")
                {
                    profile.ProfileId = Guid.CreateVersion7().ToString();
                }

                Profiles.Add(profile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load profile from {File}", file);
            }
        }

        if (Profiles.Count == 0)
        {
            var defaultProfile = new MacroDeckProfile
            {
                DisplayName = LanguageManager.Strings.Profile + " 1",
                Columns = 5,
                Rows = 3,
                Folders = []
            };

            Profiles.Add(defaultProfile);

            Save();
        }

        if (!string.IsNullOrWhiteSpace(Settings.Default.SelectedProfile))
        {
            CurrentProfile = FindProfileById(Settings.Default.SelectedProfile);
        }

        if (CurrentProfile == null)
        {
            CurrentProfile = Profiles.FirstOrDefault();
            if (CurrentProfile != null)
            {
                Settings.Default.SelectedProfile = CurrentProfile.ProfileId;
            }

            Settings.Default.Save();
        }


        if (CurrentProfile != null && CurrentProfile.Folders.Count < 1)
        {
            var root = new MacroDeckFolder
            {
                FolderId = Guid.CreateVersion7().ToString(),
                DisplayName = "*Root*",
                Childs = [],
                ActionButtons = []
            };

            CurrentProfile.Folders.Add(root);

            Save();
        }

        // Set the action button instances to the plugin actions
        foreach (var actionButton in from macroDeckProfile in Profiles
            from macroDeckFolder in macroDeckProfile.Folders
            from actionButton in macroDeckFolder.ActionButtons
            select actionButton)
        {
            actionButton.UpdateBindingState();
            actionButton.UpdateHotkey();
            UpdateVariableLabels(actionButton);
            foreach (var pluginAction in actionButton.Actions)
            {
                pluginAction?.SetActionButton(actionButton);
            }
        }

        Logger.Information("Loaded {ProfileCount} profiles", Profiles.Count);
    }

    /// <summary>
    /// 将所有配置文件序列化为 JSON 并写入磁盘。使用原子写入策略（先写临时文件再移动）
    /// 防止写入过程中崩溃导致文件损坏。同时清理已删除配置对应的孤儿 JSON 文件。
    /// 安全模式下不执行任何保存操作。
    /// </summary>
    public static void Save()
    {
        if (MacroDeck.SafeMode)
        {
            return;
        }

        lock (SaveLock)
        {
            var profilesDir = ApplicationPaths.ProfilesDirectoryPath;
            var activeIds = new HashSet<string>();

            foreach (var profile in Profiles)
            {
                var profileId = profile.ProfileId;
                activeIds.Add(profileId);

                string jsonString;
                try
                {
                    jsonString = JsonConvert.SerializeObject(profile,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            NullValueHandling = NullValueHandling.Ignore,
                            Error = (sender, args) =>
                            {
                                Logger.Error(args.ErrorContext.Error,
                                    "Error while serializing profile {ProfileId}",
                                    profileId);
                                args.ErrorContext.Handled = true;
                            },
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            Formatting = Formatting.Indented
                        });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to serialize profile {ProfileId}", profileId);
                    continue;
                }

                var finalPath = Path.Combine(profilesDir, $"{profileId}.json");
                var tmpPath = finalPath + ".tmp";
                File.WriteAllText(tmpPath, jsonString);
                File.Move(tmpPath, finalPath, overwrite: true);
            }

            foreach (var file in Directory.GetFiles(profilesDir, "*.json"))
            {
                var id = Path.GetFileNameWithoutExtension(file);
                if (!activeIds.Contains(id))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning(ex, "Failed to delete orphaned profile file {File}", file);
                    }
                }
            }
        }

        Logger.Debug("Saved {ProfileCount} profiles", Profiles.Count);
        ProfilesSaved?.Invoke(Profiles, EventArgs.Empty);
    }

    /// <summary>
    /// 将旧版 SQLite 数据库中的配置文件迁移到 JSON 文件格式。
    /// 迁移完成后将原始数据库文件重命名为 .migrated 后缀以标记已完成。
    /// 仅在旧版数据库文件存在且 Profiles 目录为空时执行。
    /// </summary>
    private static void MigrateLegacyDatabase()
    {
        var legacyPath = ApplicationPaths.ProfilesLegacyFilePath;
        var profilesDir = ApplicationPaths.ProfilesDirectoryPath;

        if (!File.Exists(legacyPath))
        {
            return;
        }

        if (Directory.GetFiles(profilesDir, "*.json").Length > 0)
        {
            return;
        }

        Logger.Information("Migrating profiles from legacy SQLite database...");

        try
        {
            var db = new SQLiteConnection(legacyPath);
            db.CreateTable<ProfileJson>();
            var entries = db.Table<ProfileJson>().ToList();
            db.Close();

            var migrated = 0;
            foreach (var entry in entries)
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore,
                        Error = (sender, args) =>
                        {
                            Logger.Error(args.ErrorContext.Error, "Error while deserializing legacy profile entry");
                            args.ErrorContext.Handled = true;
                        },
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    var profile = JsonConvert.DeserializeObject<MacroDeckProfile>(entry.JsonString, settings);
                    if (profile is null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(profile.ProfileId))
                    {
                        profile.ProfileId = migrated.ToString();
                    }

                    var finalPath = Path.Combine(profilesDir, $"{profile.ProfileId}.json");
                    var tmpPath = finalPath + ".tmp";
                    File.WriteAllText(tmpPath, entry.JsonString);
                    File.Move(tmpPath, finalPath, overwrite: true);
                    migrated++;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to migrate legacy profile entry");
                }
            }

            var migratedDbPath = legacyPath + ".migrated";
            File.Move(legacyPath, migratedDbPath, overwrite: true);
            Logger.Information(
                "Migrated {Count} profiles from SQLite to JSON files. Legacy database renamed to {MigratedPath}",
                migrated,
                migratedDbPath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to migrate legacy profiles database");
        }
    }

    /// <summary>
    /// 在指定配置文件中创建一个新文件夹。检查显示名称是否已存在，
    /// 若存在则返回 null。新文件夹会被添加到父文件夹的子文件夹列表中。
    /// </summary>
    /// <param name="displayName">文件夹的显示名称。</param>
    /// <param name="parent">父文件夹，新文件夹将作为其子文件夹。</param>
    /// <param name="macroDeckProfile">目标配置文件。</param>
    /// <returns>创建成功返回新文件夹实例，名称冲突时返回 null。</returns>
    public static MacroDeckFolder? CreateFolder(string displayName,
        MacroDeckFolder parent,
        MacroDeckProfile macroDeckProfile)
    {
        if (macroDeckProfile.Folders.FindAll(macroDeckFolder => macroDeckFolder.DisplayName.Equals(displayName)).Count >
            0)
        {
            return null;
        }

        var newFolder = new MacroDeckFolder
        {
            DisplayName = displayName,
            Childs = [],
            ActionButtons = [],
            FolderId = Guid.CreateVersion7().ToString()
        };

        parent.Childs.Add(newFolder.FolderId);

        macroDeckProfile.Folders.Add(newFolder);

        Logger.Information("Created folder {FolderName} in {ProfileName}", displayName, macroDeckProfile.DisplayName);

        Save();

        return newFolder;
    }


    /// <summary>
    /// 在指定配置文件中查找包含给定文件夹 ID 的父文件夹。
    /// </summary>
    /// <param name="macroDeckFolder">需要查找父文件夹的子文件夹。</param>
    /// <param name="macroDeckProfile">目标配置文件。</param>
    /// <returns>找到的父文件夹，未找到则返回 null。</returns>
    public static MacroDeckFolder FindParentFolder(MacroDeckFolder macroDeckFolder, MacroDeckProfile macroDeckProfile)
    {
        MacroDeckFolder parentFolder = null;
        parentFolder = macroDeckProfile.Folders.Find(folder => folder.Childs.Contains(macroDeckFolder.FolderId));
        return parentFolder;
    }

    /// <summary>
    /// 从指定配置文件中删除一个文件夹及其所有子文件夹。
    /// 根文件夹（配置中的第一个文件夹）不可删除。删除前会：
    /// 1. 释放文件夹内所有按钮的资源；
    /// 2. 将正在查看该文件夹的客户端切换到根文件夹；
    /// 3. 递归删除所有子文件夹。
    /// </summary>
    /// <param name="folder">要删除的文件夹实例。</param>
    /// <param name="macroDeckProfile">目标配置文件。</param>
    public static void DeleteFolder(MacroDeckFolder folder, MacroDeckProfile macroDeckProfile)
    {
        if (!macroDeckProfile.Folders.Contains(folder))
        {
            return;
        }

        if (macroDeckProfile.Folders.FirstOrDefault()!.Equals(folder))
        {
            return;
        }

        if (folder.ActionButtons != null)
        {
            foreach (var actionButton in folder.ActionButtons)
            {
                actionButton.Dispose();
            }
        }

        foreach (var macroDeckClient in MacroDeckServer.Clients.FindAll(macroDeckClient =>
            macroDeckClient.Folder.FolderId.Equals(folder.FolderId)))
        {
            MacroDeckServer.SetFolder(macroDeckClient, macroDeckProfile.Folders[0]);
        }

        foreach (var child in folder.Childs.Select(childId => FindFolderById(childId, macroDeckProfile)).ToArray())
        {
            DeleteFolder(child, macroDeckProfile);
        }

        foreach (var folders in macroDeckProfile.Folders.FindAll(macroDeckFolder =>
            macroDeckFolder.Childs.Contains(folder.FolderId)))
        {
            folders.Childs.Remove(folder.FolderId);
        }

        Logger.Information("Delete {FolderName} in {ProfileName}", folder.DisplayName, macroDeckProfile.DisplayName);

        macroDeckProfile.Folders.Remove(folder);
        Save();
    }


    /// <summary>
    /// 创建一个新的配置文件。如果显示名称已存在，返回已有的配置文件实例。
    /// 新配置默认包含 5 列 x 3 行的布局和一个根文件夹。
    /// </summary>
    /// <param name="displayName">新配置文件的显示名称。</param>
    /// <param name="deviceClass">目标设备类别，默认为 <see cref="DeviceClass.SoftwareClient"/>。</param>
    /// <returns>创建成功返回新配置文件实例，名称冲突时返回已有的配置文件。</returns>
    public static MacroDeckProfile CreateProfile(string displayName,
        DeviceClass deviceClass = DeviceClass.SoftwareClient)
    {
        if (Profiles.FindAll(macroDeckProfile => macroDeckProfile.DisplayName.Equals(displayName)).Count > 0)
        {
            return Profiles.Find(macroDeckProfile => macroDeckProfile.DisplayName.Equals(displayName));
        }

        var newProfile = new MacroDeckProfile
        {
            DisplayName = displayName,
            Rows = 3,
            Columns = 5,
            ButtonRadius = 40,
            ButtonSpacing = 15,
            ButtonBackground = true,
            Folders = [],
            ProfileId = Guid.CreateVersion7().ToString()
        };

        var rootFolder = new MacroDeckFolder
        {
            DisplayName = "*Root*",
            FolderId = Guid.CreateVersion7().ToString(),
            Childs = [],
            ActionButtons = []
        };

        newProfile.Folders.Add(rootFolder);

        Profiles.Add(newProfile);

        Save();

        ProfileCreated?.Invoke(newProfile, EventArgs.Empty);

        Logger.Information("Created profile {ProfileName}", displayName);

        return newProfile;
    }

    /// <summary>
    /// 删除指定的配置文件及其所有文件夹和按钮资源。
    /// 至少保留一个配置文件（Profiles.Count < 2 时拒绝删除）。
    /// 删除前会将使用该配置的设备切换到默认配置。
    /// </summary>
    /// <param name="macroDeckProfile">要删除的配置文件实例。</param>
    public static void DeleteProfile(MacroDeckProfile macroDeckProfile)
    {
        if (!Profiles.Contains(macroDeckProfile))
        {
            return;
        }

        if (Profiles.Count < 2)
        {
            return;
        }

        foreach (var macroDeckFolder in macroDeckProfile.Folders)
        {
            macroDeckFolder.Dispose();
        }

        foreach (var macroDeckDevice in DeviceManager.GetKnownDevices().FindAll(macroDeckDevice =>
            macroDeckDevice.ProfileId != null && macroDeckDevice.ProfileId.Equals(macroDeckProfile.ProfileId)))
        {
            DeviceManager.SetProfile(macroDeckDevice, Profiles[0]);
        }

        Logger.Information("Delete profile {ProfileName}", macroDeckProfile.DisplayName);

        macroDeckProfile.Dispose();

        Profiles.Remove(macroDeckProfile);

        Save();
    }

    /// <summary>
    /// 在指定配置文件中通过文件夹 ID 查找文件夹。
    /// </summary>
    /// <param name="id">文件夹的唯一标识符。</param>
    /// <param name="macroDeckProfile">目标配置文件。</param>
    /// <returns>找到的文件夹实例，未找到则返回 null。</returns>
    public static MacroDeckFolder? FindFolderById(string id, MacroDeckProfile macroDeckProfile)
    {
        return macroDeckProfile.Folders.FirstOrDefault(x => x.FolderId.Equals(id));
    }


    /// <summary>
    /// 在指定配置文件中通过显示名称查找文件夹。
    /// </summary>
    /// <param name="displayName">文件夹的显示名称。</param>
    /// <param name="macroDeckProfile">目标配置文件。</param>
    /// <returns>找到的文件夹实例，未找到则返回 null。</returns>
    public static MacroDeckFolder? FindFolderByDisplayName(string displayName, MacroDeckProfile macroDeckProfile)
    {
        return macroDeckProfile.Folders.FirstOrDefault(macroDeckFolder =>
            macroDeckFolder.DisplayName.Equals(displayName));
    }

    /// <summary>
    /// 在指定文件夹中通过网格坐标（行和列）查找按钮。
    /// </summary>
    /// <param name="folder">目标文件夹。</param>
    /// <param name="row">按钮所在的行索引。</param>
    /// <param name="col">按钮所在的列索引。</param>
    /// <returns>找到的按钮实例，未找到则返回 null。</returns>
    public static ActionButton.ActionButton? FindActionButton(MacroDeckFolder folder, int row, int col)
    {
        return folder.ActionButtons.FirstOrDefault(actionButton =>
            actionButton.Position_X == col && actionButton.Position_Y == row);
    }

    /// <summary>
    /// 在所有已加载的配置文件中通过 ID 查找配置文件。
    /// </summary>
    /// <param name="id">配置文件的唯一标识符。</param>
    /// <returns>找到的配置文件实例，未找到则返回 null。</returns>
    public static MacroDeckProfile? FindProfileById(string id)
    {
        return Profiles.FirstOrDefault(macroDeckProfile => macroDeckProfile.ProfileId.Equals(id));
    }

    /// <summary>
    /// 在所有已加载的配置文件中通过显示名称查找配置文件。
    /// </summary>
    /// <param name="displayName">配置文件的显示名称。</param>
    /// <returns>找到的配置文件实例，未找到则返回 null。</returns>
    public static MacroDeckProfile? FindProfileByDisplayName(string displayName)
    {
        return Profiles.FirstOrDefault(macroDeckProfile => macroDeckProfile.DisplayName.Equals(displayName));
    }
}
