using System.IO;
using System.IO.Compression;
using System.Text.Json;
using SuchByte.MacroDeck.StartupConfig;
using Serilog;
using SuchByte.MacroDeck.Utils;
using MessageBox = SuchByte.MacroDeck.GUI.CustomControls.MessageBox;

namespace SuchByte.MacroDeck.Backup;

/// <summary>
/// 备份失败事件参数
/// </summary>
public class BackupFailedEventArgs : EventArgs
{
    /// <summary>
    /// 备份失败的错误消息
    /// </summary>
    public string Message;
}

/// <summary>
/// 备份管理器，负责 Macro Deck 数据的备份创建、恢复和删除。
/// 备份以 ZIP 格式存储，包含配置文件、配置文件、设备信息、变量、插件、图标包等数据。
/// 恢复操作在重启后执行，通过临时目录中的 .restore 标记文件传递恢复信息。
/// </summary>
public class BackupManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(BackupManager));

    /// <summary>
    /// 标记是否有备份操作正在进行，防止并发备份
    /// </summary>
    private static bool BackupInProgress;

    /// <summary>
    /// 备份保存成功事件
    /// </summary>
    public static event EventHandler BackupSaved;

    /// <summary>
    /// 备份失败事件
    /// </summary>
    public static event EventHandler<BackupFailedEventArgs> BackupFailed;

    /// <summary>
    /// 备份删除成功事件
    /// </summary>
    public static event EventHandler DeleteSuccess;


    /// <summary>
    /// 获取所有备份文件的列表，按创建时间降序排列
    /// </summary>
    /// <returns>备份信息列表</returns>
    public static List<MacroDeckBackupInfo> GetBackups()
    {
        var backups = new List<MacroDeckBackupInfo>();
        foreach (var filename in Directory.GetFiles(ApplicationPaths.BackupsDirectoryPath))
        {
            backups.Add(new MacroDeckBackupInfo
            {
                BackupCreated = File.GetCreationTime(filename),
                FileName = filename,
                SizeMb = new FileInfo(filename).Length / 1048576.0f
            });
        }

        return backups.OrderByDescending(x => x.BackupCreated).ToList();
    }

    /// <summary>
    /// 检查恢复目录，如果存在 .restore 标记文件则执行备份恢复操作。
    /// 此方法在应用启动时调用，根据恢复信息逐项恢复配置、配置文件、设备、变量、插件等数据。
    /// </summary>
    public static void CheckRestoreDirectory()
    {
        var restoreDirectory = Path.Combine(ApplicationPaths.TempDirectoryPath, "backup_restore");
        if (!Directory.Exists(restoreDirectory))
        {
            return;
        }

        // 检查是否存在恢复标记文件
        if (!File.Exists(Path.Combine(restoreDirectory, ".restore")))
        {
            return;
        }

        // 读取恢复配置信息
        var restoreBackupInfoJson = File.ReadAllText(Path.Combine(restoreDirectory, ".restore"));
        var restoreBackupInfo = JsonSerializer.Deserialize<RestoreBackupInfo>(restoreBackupInfoJson);

        if (restoreBackupInfo == null)
        {
            return;
        }


        // 恢复主配置文件
        if (restoreBackupInfo.RestoreConfig &&
            File.Exists(Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.MainConfigFilePath))))
        {
            try
            {
                File.Copy(
                    Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.MainConfigFilePath)),
                    ApplicationPaths.MainConfigFilePath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（主配置）恢复失败");
            }
        }

        // 恢复配置文件（包含按钮布局等）
        if (restoreBackupInfo.RestoreProfiles &&
            Directory.Exists(Path.Combine(restoreDirectory, new DirectoryInfo(ApplicationPaths.ProfilesDirectoryPath).Name)))
        {
            try
            {
                if (Directory.Exists(ApplicationPaths.ProfilesDirectoryPath))
                {
                    Directory.Delete(ApplicationPaths.ProfilesDirectoryPath, true);
                }

                DirectoryCopy.Copy(
                    Path.Combine(restoreDirectory, new DirectoryInfo(ApplicationPaths.ProfilesDirectoryPath).Name),
                    ApplicationPaths.ProfilesDirectoryPath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（配置文件）恢复失败");
            }
        }

        // 恢复设备配置
        if (restoreBackupInfo.RestoreDevices &&
            File.Exists(Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.DevicesFilePath))))
        {
            try
            {
                File.Copy(Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.DevicesFilePath)),
                    ApplicationPaths.DevicesFilePath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（设备）恢复失败");
            }
        }

        // 恢复变量数据
        if (restoreBackupInfo.RestoreVariables &&
            File.Exists(Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.VariablesFilePath))))
        {
            try
            {
                File.Copy(Path.Combine(restoreDirectory, Path.GetFileName((string?)ApplicationPaths.VariablesFilePath)),
                    ApplicationPaths.VariablesFilePath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（变量）恢复失败");
            }
        }

        // 恢复插件文件
        if (restoreBackupInfo.RestorePlugins &&
            Directory.Exists(Path.Combine(restoreDirectory,
                new DirectoryInfo(ApplicationPaths.PluginsDirectoryPath).Name)))
        {
            try
            {
                if (Directory.Exists(ApplicationPaths.PluginsDirectoryPath))
                {
                    Directory.Delete(ApplicationPaths.PluginsDirectoryPath, true);
                }

                DirectoryCopy.Copy(Path.Combine(restoreDirectory,
                        new DirectoryInfo(ApplicationPaths.PluginsDirectoryPath).Name),
                    ApplicationPaths.PluginsDirectoryPath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（插件）恢复失败");
            }
        }

        // 恢复插件配置
        if (restoreBackupInfo.RestorePluginConfigs &&
            Directory.Exists(Path.Combine(restoreDirectory, new DirectoryInfo(ApplicationPaths.PluginConfigPath).Name)))
        {
            try
            {
                if (Directory.Exists(ApplicationPaths.PluginConfigPath))
                {
                    Directory.Delete(ApplicationPaths.PluginConfigPath, true);
                }

                DirectoryCopy.Copy(
                    Path.Combine(restoreDirectory, new DirectoryInfo(ApplicationPaths.PluginConfigPath).Name),
                    ApplicationPaths.PluginConfigPath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（插件配置）恢复失败");
            }
        }

        // 恢复插件凭据
        if (restoreBackupInfo.RestorePluginCredentials &&
            Directory.Exists(Path.Combine(restoreDirectory,
                new DirectoryInfo(ApplicationPaths.PluginCredentialsPath).Name)))
        {
            try
            {
                if (Directory.Exists(ApplicationPaths.PluginCredentialsPath))
                {
                    Directory.Delete(ApplicationPaths.PluginCredentialsPath, true);
                }

                DirectoryCopy.Copy(Path.Combine(restoreDirectory,
                        new DirectoryInfo(ApplicationPaths.PluginCredentialsPath).Name),
                    ApplicationPaths.PluginCredentialsPath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（插件凭据）恢复失败");
            }
        }

        // 恢复图标包
        if (restoreBackupInfo.RestoreIconPacks &&
            Directory.Exists(Path.Combine(restoreDirectory,
                new DirectoryInfo(ApplicationPaths.IconPackDirectoryPath).Name)))
        {
            try
            {
                if (Directory.Exists(ApplicationPaths.IconPackDirectoryPath))
                {
                    Directory.Delete(ApplicationPaths.IconPackDirectoryPath, true);
                }

                DirectoryCopy.Copy(Path.Combine(restoreDirectory,
                        new DirectoryInfo(ApplicationPaths.IconPackDirectoryPath).Name),
                    ApplicationPaths.IconPackDirectoryPath,
                    true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份（图标包）恢复失败");
            }
        }

        Logger.Information("备份恢复成功");
        using var msgBox = new MessageBox();
        msgBox.ShowDialog("备份已恢复", "备份恢复成功", MessageBoxButtons.OK);
    }

    /// <summary>
    /// 从指定的备份文件恢复数据。
    /// 将备份 ZIP 解压到临时目录，写入恢复信息标记文件，然后重启应用程序以完成恢复。
    /// </summary>
    /// <param name="backupFileName">备份文件名</param>
    /// <param name="restoreBackupInfo">恢复选项，指定需要恢复哪些数据</param>
    public static void RestoreBackup(string backupFileName, RestoreBackupInfo restoreBackupInfo)
    {
        var restoreDirectory = Path.Combine(ApplicationPaths.TempDirectoryPath, "backup_restore");
        try
        {
            // 准备恢复目录：创建新目录或清空已有目录
            if (!Directory.Exists(restoreDirectory))
            {
                Directory.CreateDirectory(restoreDirectory);
            }
            else
            {
                var directory = new DirectoryInfo(restoreDirectory);
                foreach (var file in directory.GetFiles())
                {
                    file.Delete();
                }

                foreach (var subDirectory in directory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "备份恢复操作失败");
        }

        try
        {
            // 解压备份文件到恢复目录
            ZipFile.ExtractToDirectory(Path.Combine(ApplicationPaths.BackupsDirectoryPath, backupFileName),
                restoreDirectory,
                true);

            // 写入恢复信息标记文件，应用重启后将读取此文件执行恢复
            var backupInfoSerialized = JsonSerializer.Serialize(restoreBackupInfo);
            File.WriteAllText(Path.Combine(restoreDirectory, ".restore"), backupInfoSerialized);

            // 重启应用以完成恢复
            MacroDeck.RestartMacroDeck("--show");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "备份恢复操作失败");
        }
    }


    /// <summary>
    /// 创建完整备份，将配置文件、配置文件、设备信息、变量、插件、插件配置、凭据和图标包打包为 ZIP 文件。
    /// 使用防重入标志避免并发备份操作。
    /// </summary>
    public static void CreateBackup()
    {
        if (BackupInProgress)
        {
            return;
        }

        BackupInProgress = true;
        var backupFileName = $"backup_{DateTime.Now:yy-MM-dd_HH-mm-ss}.zip";
        Logger.Information("开始创建备份：{BackupFileName}", backupFileName);

        var tempBackupPath = Path.Combine(ApplicationPaths.TempDirectoryPath, backupFileName);
        try
        {
            Directory.CreateDirectory(tempBackupPath);

            // 复制核心配置文件到临时目录
            File.Copy(ApplicationPaths.MainConfigFilePath,
                Path.Combine(tempBackupPath, Path.GetFileName((string?)ApplicationPaths.MainConfigFilePath)));
            File.Copy(ApplicationPaths.DevicesFilePath,
                Path.Combine(tempBackupPath, Path.GetFileName((string?)ApplicationPaths.DevicesFilePath)));
            File.Copy(ApplicationPaths.VariablesFilePath,
                Path.Combine(tempBackupPath, Path.GetFileName((string?)ApplicationPaths.VariablesFilePath)));

            // 将临时目录中的文件打包为 ZIP
            CreateBackup(backupFileName, tempBackupPath);

            Logger.Information("备份创建成功：{BackupFileName}", backupFileName);
            BackupSaved?.Invoke(null, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "备份创建失败");
            BackupFailed?.Invoke(null, new BackupFailedEventArgs { Message = ex.Message });
        }
        finally
        {
            BackupInProgress = false;
        }
    }

    /// <summary>
    /// 将指定临时目录中的文件和各子目录数据打包为 ZIP 备份文件。
    /// 包含：核心文件、配置文件、插件目录、插件配置、插件凭据、图标包目录。
    /// </summary>
    /// <param name="backupFileName">备份 ZIP 文件名</param>
    /// <param name="tempBackupPath">临时备份文件目录</param>
    private static void CreateBackup(string backupFileName, string tempBackupPath)
    {
        using var archive = ZipFile.Open(Path.Combine(ApplicationPaths.BackupsDirectoryPath, backupFileName),
            ZipArchiveMode.Create);

        // 添加临时目录中的核心文件
        foreach (var file in Directory.GetFiles(tempBackupPath))
        {
            archive.CreateEntryFromFile(file, Path.GetFileName(file));
        }

        // 添加配置文件目录中的 JSON 文件
        var profilesDirectoryInfo = new DirectoryInfo(ApplicationPaths.ProfilesDirectoryPath);
        foreach (var profileFile in profilesDirectoryInfo.GetFiles("*.json"))
        {
            archive.CreateEntryFromFile(
                profileFile.FullName,
                Path.Combine(profilesDirectoryInfo.Name, profileFile.Name));
        }

        // 添加插件目录中的所有文件
        foreach (var directory in Directory.GetDirectories(ApplicationPaths.PluginsDirectoryPath))
        {
            var pluginDirectoryInfo = new DirectoryInfo(directory);
            foreach (var file in pluginDirectoryInfo.GetFiles("*"))
            {
                archive.CreateEntryFromFile(
                    Path.Combine(ApplicationPaths.PluginsDirectoryPath, pluginDirectoryInfo.Name, file.Name),
                    Path.Combine(new DirectoryInfo(ApplicationPaths.PluginsDirectoryPath).Name,
                        pluginDirectoryInfo.Name,
                        file.Name));
            }
        }

        // 添加插件配置目录中的文件
        var pluginConfigDirectoryInfo = new DirectoryInfo(ApplicationPaths.PluginConfigPath);
        foreach (var file in pluginConfigDirectoryInfo.GetFiles("*"))
        {
            archive.CreateEntryFromFile(Path.Combine(ApplicationPaths.PluginConfigPath, file.Name),
                Path.Combine(pluginConfigDirectoryInfo.Name, file.Name));
        }

        // 添加插件凭据目录中的文件
        var pluginCredentialsDirectoryInfo = new DirectoryInfo(ApplicationPaths.PluginCredentialsPath);
        foreach (var file in pluginCredentialsDirectoryInfo.GetFiles("*"))
        {
            archive.CreateEntryFromFile(Path.Combine(ApplicationPaths.PluginCredentialsPath, file.Name),
                Path.Combine(pluginCredentialsDirectoryInfo.Name, file.Name));
        }

        // 添加图标包目录中的所有文件
        var iconPackDirectoryInfo = new DirectoryInfo(ApplicationPaths.IconPackDirectoryPath);
        foreach (var dir in iconPackDirectoryInfo.GetDirectories())
        {
            foreach (var iconPackFile in dir.GetFiles())
            {
                archive.CreateEntryFromFile(
                    Path.Combine(ApplicationPaths.IconPackDirectoryPath, dir.Name, iconPackFile.Name),
                    Path.Combine(iconPackDirectoryInfo.Name, dir.Name, iconPackFile.Name));
            }
        }
    }

    /// <summary>
    /// 删除指定的备份文件
    /// </summary>
    /// <param name="fileName">要删除的备份文件完整路径</param>
    public static void DeleteBackup(string fileName)
    {
        if (File.Exists(fileName))
        {
            try
            {
                File.Delete(fileName);
                Logger.Information("备份删除成功：{FileName}", fileName);
                DeleteSuccess?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "备份删除失败");
            }
        }
    }
}
