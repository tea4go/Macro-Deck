using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// 插件清单类，包含插件的元数据信息。
/// 已过时，将在 2.11.0 版本中移除。
/// </summary>
[Obsolete("Will be removed in version 2.11.0")]
public class PluginManifest
{
    /// <summary>插件包标识</summary>
    public string PackageId = "";

    /// <summary>插件作者</summary>
    public string Author = "";

    /// <summary>插件仓库地址</summary>
    public string Repository = "";

    /// <summary>插件主文件名，例如 "MyPlugin.dll"</summary>
    public string MainFile = "";

    /// <summary>
    /// 从清单文件加载插件清单
    /// </summary>
    /// <param name="path">清单文件路径</param>
    /// <returns>插件清单实例</returns>
    public static PluginManifest FromManifestFile(string path)
    {
        using var s = File.OpenRead(path);
        return FromXmlStream(s);
    }

    /// <summary>
    /// 从 ZIP 文件中加载插件清单（查找 Plugin.xml 条目）
    /// </summary>
    /// <param name="zipFilePath">ZIP 文件路径</param>
    /// <returns>插件清单实例</returns>
    public static PluginManifest FromZipFilePath(string zipFilePath)
    {
        var stream = new StreamReader(ZipFile.OpenRead(zipFilePath)
                .Entries.Where(x => x.Name.Equals("Plugin.xml", StringComparison.InvariantCulture))
                .FirstOrDefault()
                .Open(),
            Encoding.UTF8).BaseStream;
        return FromXmlStream(stream);
    }

    /// <summary>
    /// 从 XML 流反序列化插件清单
    /// </summary>
    /// <param name="stream">XML 流</param>
    /// <returns>插件清单实例</returns>
    public static PluginManifest FromXmlStream(Stream stream)
    {
        return (PluginManifest)new XmlSerializer(typeof(PluginManifest)).Deserialize(stream);
    }
}
