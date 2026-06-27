using System.IO;
using Newtonsoft.Json;
using Serilog;
using SuchByte.MacroDeck.StartupConfig;
using SuchByte.MacroDeck.Utils;

namespace SuchByte.MacroDeck.Plugins;

/// <summary>
/// 插件凭据管理器，负责插件敏感数据（如 API 密钥、令牌）的加密存储和读取。
/// 使用机器 GUID 作为加密密钥，确保凭据与当前机器绑定。
/// 凭据以加密形式存储在磁盘上，读取时自动解密。
/// </summary>
public class PluginCredentials
{
    private static readonly ILogger Logger = Log.ForContext(typeof(PluginCredentials));

    /// <summary>
    /// 生成凭据文件名（格式：作者名_插件名，全小写）
    /// </summary>
    private static string FileName(MacroDeckPlugin plugin)
    {
        return $"{plugin.Author.ToLower()}_{plugin.Name.ToLower()}";
    }

    /// <summary>
    /// 获取凭据文件的完整路径
    /// </summary>
    private static string FilePath(MacroDeckPlugin plugin)
    {
        return Path.Combine(ApplicationPaths.PluginCredentialsPath, FileName(plugin));
    }

    /// <summary>
    /// 将加密后的凭据列表保存到磁盘
    /// </summary>
    private static void Save(MacroDeckPlugin plugin, List<Dictionary<string, string>> pluginCredentials)
    {
        var serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        try
        {
            using var sw = new StreamWriter(FilePath(plugin));
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, pluginCredentials);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "添加插件凭据时发生错误");
        }
    }

    /// <summary>
    /// 添加凭据。将键值对使用机器 GUID 加密后追加到凭据列表。
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <param name="keyValuePairs">要添加的凭据键值对</param>
    public static void AddCredentials(MacroDeckPlugin plugin, Dictionary<string, string> keyValuePairs)
    {
        var keyValuePairsEncrypted = new Dictionary<string, string>();

        // 使用机器 GUID 加密每个值
        foreach (var entry in keyValuePairs)
        {
            keyValuePairsEncrypted[entry.Key] = StringCipher.Encrypt(entry.Value, StringCipher.GetMachineGuid());
        }

        var pluginCredentials = GetEncryptedCredentials(plugin);

        pluginCredentials.Add(keyValuePairsEncrypted);

        Save(plugin, pluginCredentials);
    }

    /// <summary>
    /// 设置凭据（覆盖现有凭据）。将键值对加密后替换整个凭据列表。
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <param name="keyValuePairs">要设置的凭据键值对</param>
    public static void SetCredentials(MacroDeckPlugin plugin, Dictionary<string, string> keyValuePairs)
    {
        var keyValuePairsEncrypted = new Dictionary<string, string>();

        foreach (var entry in keyValuePairs)
        {
            keyValuePairsEncrypted[entry.Key] = StringCipher.Encrypt(entry.Value, StringCipher.GetMachineGuid());
        }

        List<Dictionary<string, string>> pluginCredentials = new();
        pluginCredentials.Add(keyValuePairsEncrypted);

        Save(plugin, pluginCredentials);
    }

    /// <summary>
    /// 删除插件的凭据文件
    /// </summary>
    /// <param name="plugin">目标插件</param>
    public static void DeletePluginCredentials(MacroDeckPlugin plugin)
    {
        File.Delete(FilePath(plugin));
    }

    /// <summary>
    /// 获取插件的解密凭据列表。
    /// 使用机器 GUID 解密每个值。如果解密失败（如机器 GUID 变更），记录警告。
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <returns>解密后的凭据列表</returns>
    public static List<Dictionary<string, string>> GetPluginCredentials(MacroDeckPlugin plugin)
    {
        var pluginCredentialsEncrypted = GetEncryptedCredentials(plugin);

        var pluginCredentialsDecrypted = new List<Dictionary<string, string>>();

        if (pluginCredentialsEncrypted == null)
        {
            return pluginCredentialsDecrypted;
        }

        foreach (var pluginCredentialEncrypted in pluginCredentialsEncrypted)
        {
            var pluginCredentialDecrypted = new Dictionary<string, string>();
            foreach (var entry in pluginCredentialEncrypted)
            {
                try
                {
                    pluginCredentialDecrypted[entry.Key] =
                        StringCipher.Decrypt(entry.Value, StringCipher.GetMachineGuid());
                }
                catch
                {
                    Logger.Warning("无法解密插件 {PluginName} 的凭据，可能机器 GUID 已变更",
                        plugin.Name);
                }
            }

            pluginCredentialsDecrypted.Add(pluginCredentialDecrypted);
        }

        return pluginCredentialsDecrypted;
    }

    /// <summary>
    /// 从磁盘读取加密的凭据数据
    /// </summary>
    /// <param name="plugin">目标插件</param>
    /// <returns>加密的凭据列表，文件不存在时返回空列表</returns>
    private static List<Dictionary<string, string>>? GetEncryptedCredentials(MacroDeckPlugin plugin)
    {
        if (!File.Exists(FilePath(plugin)))
        {
            return new List<Dictionary<string, string>>();
        }

        return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(File.ReadAllText(FilePath(plugin)),
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
    }
}
