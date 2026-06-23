using SQLite;

namespace SuchByte.MacroDeck.JSON;

/// <summary>
/// 图标包 JSON 数据模型，用于 SQLite 数据库存储图标包的 JSON 序列化数据。
/// </summary>
public class IconPackJson
{
    /// <summary>主键 ID，自增</summary>
    [PrimaryKey] [AutoIncrement] public int Id { get; }

    /// <summary>图标包的 JSON 序列化字符串</summary>
    public string JsonString { get; set; }
}
