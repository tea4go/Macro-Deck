using SQLite;

namespace SuchByte.MacroDeck.JSON;

/// <summary>
/// 文件夹 JSON 数据模型，用于 SQLite 数据库存储文件夹的 JSON 序列化数据。
/// </summary>
public class FolderJson
{
    /// <summary>主键 ID，自增</summary>
    [PrimaryKey] [AutoIncrement] public int Id { get; }

    /// <summary>文件夹的 JSON 序列化字符串</summary>
    public string JsonString { get; set; }
}
