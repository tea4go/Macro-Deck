using System.IO;
using Serilog;

namespace SuchByte.MacroDeck.StartupConfig;

/// <summary>
/// 应用程序路径管理器。
/// 管理 Macro Deck 的所有目录和文件路径，支持便携模式和标准安装模式。
/// 提供路径初始化、检查创建和临时目录清理功能。
/// </summary>
public class ApplicationPaths 
{ 
    /// <summary>
    /// Serilog 日志记录器实例，用于路径操作相关的日志记录。
    /// </summary>
    private static readonly ILogger Logger = Log.ForContext(typeof(ApplicationPaths)); 
 
    private static bool _portableMode; 
 
    /// <summary>当前进程的可执行文件路径。</summary> 
    public static string ExecutablePath { get; private set; } 
    /// <summary>应用程序主目录路径（即程序集所在目录）。</summary> 
    public static string MainDirectoryPath { get; } 
    /// <summary>用户数据目录路径。便携模式下为 Data 子目录，标准模式下为用户 AppData 目录。</summary> 
    public static string UserDirectoryPath { get; private set; } = null!; 
    /// <summary>插件目录路径。</summary> 
    public static string PluginsDirectoryPath { get; private set; } = null!; 
    /// <summary>插件更新临时目录路径。</summary> 
    public static string UpdatePluginsDirectoryPath { get; private set; } = null!; 
    /// <summary>临时文件目录路径。</summary> 
    public static string TempDirectoryPath { get; private set; } = null!; 
    /// <summary>图标包目录路径。</summary> 
    public static string IconPackDirectoryPath { get; private set; } = null!; 
    /// <summary>插件凭据存储目录路径。</summary> 
    public static string PluginCredentialsPath { get; private set; } = null!; 
    /// <summary>插件配置文件目录路径。</summary> 
    public static string PluginConfigPath { get; private set; } = null!; 
    /// <summary>备份文件目录路径。</summary> 
    public static string BackupsDirectoryPath { get; private set; } = null!; 
    /// <summary>日志文件目录路径。</summary> 
    public static string LogsDirectoryPath { get; private set; } = null!; 
    /// <summary>主配置文件路径（config.json）。</summary> 
    public static string MainConfigFilePath { get; private set; } = null!; 
    /// <summary>设备配置文件路径（devices.json）。</summary> 
    public static string DevicesFilePath { get; private set; } = null!; 
    /// <summary>变量数据库文件路径（variables.db）。</summary> 
    public static string VariablesFilePath { get; private set; } = null!; 
    /// <summary>旧版配置文件路径（profiles.db）。</summary> 
    public static string ProfilesLegacyFilePath { get; private set; } = null!; 
    /// <summary>配置文件目录路径。</summary> 
    public static string ProfilesDirectoryPath { get; private set; } = null!; 
 
    /// <summary>静态构造函数。初始化可执行文件路径和主目录路径。</summary> 
    static ApplicationPaths() 
    { 
        ExecutablePath = Environment.ProcessPath ?? 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Macro Deck 2.exe"); 
        MainDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; 
    } 
 
    /// <summary>初始化应用程序路径系统。根据运行模式设置路径并检查创建所需目录。</summary> 
    /// <param name="portableMode">是否以便携模式运行。</param> 
    public static void Initialize(bool portableMode) 
    { 
        _portableMode = portableMode; 
        InitializePaths(); 
        CheckPaths(); 
    } 
 
    /// <summary>根据当前运行模式初始化所有目录和文件路径。便携模式下数据目录位于应用程序目录下的 Data 文件夹中；标准模式下数据目录位于用户的 AppData 文件夹中。</summary> 
    private static void InitializePaths() 
    { 
        UserDirectoryPath = _portableMode 
            ? Path.Combine(MainDirectoryPath, "Data") 
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Macro Deck"); 
        PluginsDirectoryPath = Path.Combine(UserDirectoryPath, "plugins"); 
        UpdatePluginsDirectoryPath = Path.Combine(PluginsDirectoryPath, ".updates"); 
        TempDirectoryPath = Path.Combine(UserDirectoryPath, ".temp"); 
        IconPackDirectoryPath = Path.Combine(UserDirectoryPath, "iconpacks"); 
        PluginCredentialsPath = Path.Combine(UserDirectoryPath, "credentials"); 
        PluginConfigPath = Path.Combine(UserDirectoryPath, "configs"); 
        BackupsDirectoryPath = Path.Combine(UserDirectoryPath, "backups"); 
        LogsDirectoryPath = Path.Combine(UserDirectoryPath, "logs"); 
        MainConfigFilePath = Path.Combine(UserDirectoryPath, "config.json"); 
        DevicesFilePath = Path.Combine(UserDirectoryPath, "devices.json"); 
        VariablesFilePath = Path.Combine(UserDirectoryPath, "variables.db"); 
        ProfilesLegacyFilePath = Path.Combine(UserDirectoryPath, "profiles.db"); 
        ProfilesDirectoryPath = Path.Combine(UserDirectoryPath, "profiles"); 
    } 
 
    /// <summary>检查所有必要的目录是否存在，若不存在则创建。</summary> 
    private static void CheckPaths() 
    { 
        Logger.Information("正在检查路径…"); 
 
        void CheckCreatePath(string? path) 
        { 
            if (string.IsNullOrWhiteSpace(path)) 
            { 
                return; 
            } 
 
            if (Directory.Exists(path)) 
            { 
                return; 
            } 
 
            try 
            { 
                Directory.CreateDirectory(path); 
                Logger.Information("已创建目录：{Path}", path);
            } 
            catch (Exception ex) 
            { 
                Logger.Error(ex, "创建目录失败：{Path}", path); 
            } 
        } 
 
        CheckCreatePath(UserDirectoryPath); 
        CheckCreatePath(LogsDirectoryPath); 
        CheckCreatePath(PluginCredentialsPath); 
        CheckCreatePath(PluginConfigPath); 
        CheckCreatePath(PluginsDirectoryPath); 
        CheckCreatePath(BackupsDirectoryPath); 
        CheckCreatePath(IconPackDirectoryPath); 
        CheckCreatePath(TempDirectoryPath); 
        CheckCreatePath(ProfilesDirectoryPath); 
 
        Logger.Information("路径检查完成");
    } 
 
    /// <summary>清理临时目录中的所有文件和子目录。</summary> 
    public static void CleanUpTempDirectory() 
    { 
        DirectoryInfo di = new(TempDirectoryPath); 
        CleanupTempDir(di); 
 
 
        void CleanupTempDir(DirectoryInfo directoryInfo) 
        { 
            if (!directoryInfo.Exists) 
            { 
                return; 
            } 
 
            foreach (var file in directoryInfo.GetFiles()) 
            { 
                try 
                { 
                    file.Delete(); 
                } 
                catch (Exception ex) 
                { 
                    Logger.Warning(ex, "删除临时文件失败"); 
                } 
            } 
 
            foreach (var dir in directoryInfo.GetDirectories()) 
            { 
                try 
                { 
                    dir.Delete(true); 
                } 
                catch (Exception ex) 
                { 
                    Logger.Warning(ex, "删除临时目录失败"); 
                } 
            } 
        } 
    } 
} 
