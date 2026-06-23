using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 扩展商店下载器包信息模型，用于指定要下载安装的扩展包。
/// 包含包标识和扩展类型，传递给 ExtensionStoreDownloader 进行下载。
/// </summary>
public class ExtensionStoreDownloaderPackageInfoModel
{
    /// <summary>扩展包唯一标识</summary>
    public string PackageId { get; set; } = "";

    /// <summary>扩展类型（插件或图标包）</summary>
    public ExtensionType ExtensionType { get; set; }
}
