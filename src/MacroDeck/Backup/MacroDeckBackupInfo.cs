namespace SuchByte.MacroDeck.Backup;

/// <summary>
/// Macro Deck 备份文件信息模型
/// </summary>
public class MacroDeckBackupInfo
{
    /// <summary>
    /// 备份创建时间
    /// </summary>
    public DateTime BackupCreated;

    /// <summary>
    /// 备份文件完整路径
    /// </summary>
    public string FileName;

    /// <summary>
    /// 备份文件大小（MB）
    /// </summary>
    public float SizeMb;
}
