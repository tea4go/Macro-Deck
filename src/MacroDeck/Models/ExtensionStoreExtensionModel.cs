using Newtonsoft.Json;
using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 扩展商店扩展模型，对应扩展商店 API 返回的扩展信息。
/// 使用 Newtonsoft.Json 映射 JSON 属性名（kebab-case 到 Pascal-case）。
/// 包含扩展的包标识、类型、名称、版本、作者、仓库、文件名、目标 API 版本、MD5 校验和和图标。
/// </summary>
public class ExtensionStoreExtensionModel
{
    /// <summary>扩展包唯一标识</summary>
    [JsonProperty("package-id")] public string PackageId { get; set; } = "";

    /// <summary>扩展类型（插件或图标包）</summary>
    [JsonProperty("type")] public ExtensionType Type { get; set; } = ExtensionType.Plugin;

    /// <summary>扩展名称</summary>
    [JsonProperty("name")] public string Name { get; set; } = "";

    /// <summary>扩展版本</summary>
    [JsonProperty("version")] public string Version { get; set; } = "";

    /// <summary>扩展作者</summary>
    [JsonProperty("author")] public string Author { get; set; } = "";

    /// <summary>扩展仓库地址</summary>
    [JsonProperty("repository")] public string Repository { get; set; } = "";

    /// <summary>扩展安装包文件名</summary>
    [JsonProperty("filename")] public string Filename { get; set; } = "";

    /// <summary>目标插件 API 版本号</summary>
    [JsonProperty("target-api")] public int TargetAPI { get; set; }

    /// <summary>安装包 MD5 校验和，用于验证文件完整性</summary>
    [JsonProperty("md5")] public string Md5Checksum { get; set; } = "";

    /// <summary>扩展图标（Base64 编码）</summary>
    [JsonProperty("icon")] public string IconBase64 { get; set; } = "";
}
