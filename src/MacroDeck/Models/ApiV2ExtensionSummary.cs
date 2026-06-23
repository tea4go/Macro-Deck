using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// API V2 扩展摘要模型，对应扩展商店 API V2 返回的扩展概要信息。
/// 用于扩展商店搜索和列表展示。
/// </summary>
public class ApiV2ExtensionSummary
{
    /// <summary>扩展包唯一标识</summary>
    public string? PackageId { get; set; }

    /// <summary>扩展类型（插件或图标包）</summary>
    public ExtensionType? ExtensionType { get; set; }

    /// <summary>扩展名称</summary>
    public string? Name { get; set; }

    /// <summary>扩展作者</summary>
    public string? Author { get; set; }

    /// <summary>扩展描述</summary>
    public string? Description { get; set; }

    /// <summary>GitHub 仓库地址</summary>
    public string? GitHubRepository { get; set; }

    /// <summary>Discord 支持用户 ID</summary>
    public string? DSupportUserId { get; set; }
}
