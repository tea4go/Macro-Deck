using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using SuchByte.MacroDeck.ExtensionStore;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 扩展清单模型，描述插件或图标包的元数据信息。
/// 每个扩展的根目录中必须包含 ExtensionManifest.json 文件。
/// 支持从文件路径、ZIP 压缩包和流中读取清单信息。
/// </summary>
public class ExtensionManifestModel
{
    /// <summary>扩展类型（插件或图标包）</summary>
    [JsonProperty("type")] public ExtensionType Type { get; set; } = ExtensionType.Plugin;

    /// <summary>扩展名称</summary>
    [JsonProperty("name")] public string Name { get; set; }

    /// <summary>扩展作者</summary>
    [JsonProperty("author")] public string Author { get; set; }

    /// <summary>扩展仓库地址</summary>
    [JsonProperty("repository")] public string Repository { get; set; } = "";

    /// <summary>扩展包唯一标识</summary>
    [JsonProperty("packageId")] public string PackageId { get; set; }

    /// <summary>扩展版本</summary>
    [JsonProperty("version")] public string Version { get; set; }

    /// <summary>目标插件 API 版本号，默认为 31</summary>
    [JsonProperty("target-plugin-api-version")]
    public int TargetPluginAPIVersion { get; set; } = 31;

    /// <summary>插件主 DLL 文件名</summary>
    [JsonProperty("dll")] public string Dll { get; set; } = "";

    /// <summary>
    /// 将扩展清单模型序列化为 JSON 字符串
    /// </summary>
    /// <param name="extensionManifestModel">要序列化的清单模型</param>
    /// <returns>JSON 格式的字符串</returns>
    public static string Serialize(ExtensionManifestModel extensionManifestModel)
    {
        return JsonConvert.SerializeObject(extensionManifestModel);
    }

    /// <summary>
    /// 从清单文件路径读取并反序列化扩展清单
    /// </summary>
    /// <param name="path">ExtensionManifest.json 文件路径</param>
    /// <returns>反序列化后的清单模型，失败返回 null</returns>
    public static ExtensionManifestModel? FromManifestFile(string path)
    {
        using var s = File.OpenRead(path);
        return FromJsonStream(s);
    }

    /// <summary>
    /// 从 ZIP 压缩包中读取扩展清单。
    /// 在 ZIP 文件中查找名为 "ExtensionManifest.json" 的条目并解析。
    /// </summary>
    /// <param name="zipFilePath">ZIP 文件路径</param>
    /// <returns>反序列化后的清单模型，失败返回 null</returns>
    public static ExtensionManifestModel? FromZipFilePath(string zipFilePath)
    {
        var stream = new StreamReader(ZipFile.OpenRead(zipFilePath)
                .Entries.Where(x => x.Name.Equals("ExtensionManifest.json", StringComparison.InvariantCulture))
                .FirstOrDefault()
                .Open(),
            Encoding.UTF8).BaseStream;
        return FromJsonStream(stream);
    }

    /// <summary>
    /// 从流中反序列化扩展清单
    /// </summary>
    /// <param name="stream">包含 JSON 数据的流</param>
    /// <returns>反序列化后的清单模型，流为空时返回 null</returns>
    public static ExtensionManifestModel? FromJsonStream(Stream stream)
    {
        var serializer = new JsonSerializer();
        using var sr = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(sr);
        while (!sr.EndOfStream)
        {
            return serializer.Deserialize<ExtensionManifestModel>(jsonReader);
        }

        return null;
    }
}
