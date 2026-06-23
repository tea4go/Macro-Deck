namespace SuchByte.MacroDeck.Backup;

/// <summary>
/// 备份恢复选项模型，指定恢复备份时需要恢复哪些数据项
/// </summary>
public class RestoreBackupInfo
{
    /// <summary>是否恢复主配置文件</summary>
    public bool RestoreConfig { get; set; } = false;
    /// <summary>是否恢复配置文件（按钮布局等）</summary>
    public bool RestoreProfiles { get; set; } = false;
    /// <summary>是否恢复设备配置</summary>
    public bool RestoreDevices { get; set; } = false;
    /// <summary>是否恢复变量数据</summary>
    public bool RestoreVariables { get; set; } = false;
    /// <summary>是否恢复插件文件</summary>
    public bool RestorePlugins { get; set; } = false;
    /// <summary>是否恢复插件配置</summary>
    public bool RestorePluginConfigs { get; set; } = false;
    /// <summary>是否恢复插件凭据</summary>
    public bool RestorePluginCredentials { get; set; } = false;
    /// <summary>是否恢复图标包</summary>
    public bool RestoreIconPacks { get; set; } = false;
}
