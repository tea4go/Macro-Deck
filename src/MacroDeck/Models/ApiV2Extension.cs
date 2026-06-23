using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// API V2 扩展详情模型，对应扩展商店 API V2 返回的扩展完整信息。
/// 包含扩展的元数据和可用文件列表，用于扩展详情页展示和下载。
/// </summary>
public class ApiV2Extension
{
    /// <summary>扩展包唯一标识</summary>
    public string? PackageId { get; set; }

    /// <summary>扩展类型（插件或图标包）</summary>
    public ExtensionType ExtensionType { get; set; }

    /// <summary>扩展名称</summary>
    public string? Name { get; set; }

    /// <summary>扩展分类</summary>
    public string? Category { get; set; }

    /// <summary>扩展作者</summary>
    public string? Author { get; set; }

    /// <summary>扩展描述</summary>
    public string? Description { get; set; }

    /// <summary>GitHub 仓库地址</summary>
    public string? GitHubRepository { get; set; }

    /// <summary>Discord 支持用户 ID</summary>
    public string? DSupportUserId { get; set; }

    /// <summary>扩展可用文件列表（不同版本/平台的安装包）</summary>
    public IList<ApiV2ExtensionFile>? ExtensionFiles { get; set; }
}
