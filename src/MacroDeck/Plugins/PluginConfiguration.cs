using System.IO;
using Newtonsoft.Json;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// 插件配置管理器，负责插件配置的键值对存储和读取。
/// 配置以 JSON 文件形式保存在插件配置目录中。
/// </summary>
public class PluginConfiguration
{
    /// <summary>
    /// 生成配置文件名（格式：作者名_插件名.json，全小写）
    /// </summary>
    private static string FileName(MacroDeckPlugin plugin)
    {
        return $"{plugin.Author.ToLower()}_{plugin.Name.ToLower()}.json";
    }

    /// <summary>
    /// 获取配置文件的完整路径
    /// </summary>
    private static string FilePath(MacroDeckPlugin plugin)
    {
        return Path.Combine(ApplicationPaths.PluginConfigPath, FileName(plugin));
    }

    /// <summary>
    /// 设置插件配置项的值。如果键已存在则更新，否则添加。
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    public static void SetValue(MacroDeckPlugin plugin, string key, string value)
    {
        try
        {
            var pluginConfig = GetConfig(plugin);

            pluginConfig[key] = value;

            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };

            using var sw = new StreamWriter(FilePath(plugin));
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, pluginConfig);
        }
        catch
        {
        }
    }

    /// <summary>
    /// 从磁盘读取插件的完整配置字典
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <returns>配置字典，文件不存在时返回空字典</returns>
    private static Dictionary<string, string>? GetConfig(MacroDeckPlugin plugin)
    {
        if (!File.Exists(FilePath(plugin)))
        {
            return new Dictionary<string, string>();
        }

        return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(FilePath(plugin)),
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Error = (sender, args) => { args.ErrorContext.Handled = true; }
            });
    }


    /// <summary>
    /// 获取插件配置项的值
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <param name="key">配置键</param>
    /// <returns>配置值，未找到时返回空字符串</returns>
    public static string GetValue(MacroDeckPlugin plugin = null, string key = "")
    {
        var value = "";
        try
        {
            if (plugin == null || key == null)
            {
                return value;
            }

            var pluginConfig = GetConfig(plugin);

            if (pluginConfig != null && !string.IsNullOrWhiteSpace(pluginConfig[key]))
            {
                value = pluginConfig[key];
            }
        }
        catch
        {
        }

        return value;
    }

    /// <summary>
    /// 删除插件的配置文件
    /// </summary>
    /// <param name="plugin">目标插件</param>
    public static void DeletePluginConfig(MacroDeckPlugin plugin)
    {
        try
        {
            File.Delete(FilePath(plugin));
        }
        catch
        {
        }
    }
}
