namespace SuchByte.MacroDeck.Icons;

/// <summary>
/// 图标包类，表示一组图标的集合。
/// 包含图标包的元数据（名称、作者、版本）和图标列表。
/// </summary>
public class IconPack
{
    /// <summary>图标包的唯一包标识</summary>
    public string PackageId;

    /// <summary>图标包名称</summary>
    public string Name;

    /// <summary>图标包作者</summary>
    public string Author;

    /// <summary>图标包版本</summary>
    public string Version;

    /// <summary>图标包中包含的所有图标列表</summary>
    public List<Icon> Icons;

    /// <summary>
    /// 图标包预览图标，在扩展管理器中显示。
    /// 不永久填充以避免每个图标包长期占用内存；
    /// 消费者通过 IconPackPreview.GeneratePreviewImage 按需生成并释放。
    /// </summary>
    public Image IconPackIcon { get; set; }

    /// <summary>
    /// 是否由扩展商店管理。为 true 时禁止编辑图标包。
    /// </summary>
    public bool ExtensionStoreManaged { get; set; } = false;

    /// <summary>
    /// 是否隐藏图标包
    /// </summary>
    public bool Hidden { get; set; } = false;
}
