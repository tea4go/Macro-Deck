namespace SuchByte.MacroDeck.Models;

/// <summary>
/// API V2 扩展文件模型，对应扩展商店 API V2 返回的扩展文件详情。
/// 包含扩展安装包的版本、文件名、哈希校验、许可证和上传时间等信息。
/// </summary>
public class ApiV2ExtensionFile
{
    /// <summary>扩展文件版本</summary>
    public string? Version { get; set; }

    /// <summary>最低 API 版本要求</summary>
    public int? MinApiVersion { get; set; }

    /// <summary>安装包文件名</summary>
    public string? PackageFileName { get; set; }

    /// <summary>图标文件名</summary>
    public string? IconFileName { get; set; }

    /// <summary>README 内容</summary>
    public string? Readme { get; set; }

    /// <summary>文件哈希校验值，用于验证下载完整性</summary>
    public string? FileHash { get; set; }

    /// <summary>许可证名称</summary>
    public string? LicenseName { get; set; }

    /// <summary>许可证 URL</summary>
    public string? LicenseUrl { get; set; }

    /// <summary>文件上传时间</summary>
    public DateTime? UploadDateTime { get; set; }
}
