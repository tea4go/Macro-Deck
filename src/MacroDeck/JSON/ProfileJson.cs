using SQLite;

namespace SuchByte.MacroDeck.JSON;

/// <summary>
/// 配置文件 JSON 数据模型，用于 SQLite 数据库存储配置的 JSON 序列化数据。
/// </summary>
public class ProfileJson
{
    /// <summary>主键 ID，自增</summary>
    [PrimaryKey] [AutoIncrement] public int Id { get; }

    /// <summary>配置的 JSON 序列化字符串</summary>
    public string JsonString { get; set; }
}
