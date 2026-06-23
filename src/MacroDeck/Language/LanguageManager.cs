using System.IO;
using Newtonsoft.Json;
using Serilog;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Language;

/// <summary>
/// 语言管理器，负责加载、管理和切换应用程序的多语言支持。
/// 从嵌入资源中加载语言文件，支持运行时切换语言。
/// </summary>
public static class LanguageManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(LanguageManager));

    /// <summary>语言变更事件</summary>
    public static event EventHandler LanguageChanged;

    /// <summary>所有可用语言列表</summary>
    public static List<Strings> Languages => _languages;

    private static List<Strings> _languages = new();

    /// <summary>当前使用的语言字符串</summary>
    private static Strings _strings = new();

    /// <summary>当前使用的语言字符串实例</summary>
    public static Strings Strings => _strings;


    /// <summary>
    /// 加载所有语言文件。从嵌入资源中读取语言 JSON 文件，
    /// 去重后按语言名称排序。可选导出默认语言文件到磁盘。
    /// </summary>
    /// <param name="exportDefaultStrings">是否导出默认语言文件到磁盘</param>
    public static void Load(bool exportDefaultStrings = false)
    {
        _languages.Clear();
        _languages.Add(_strings);
        if (exportDefaultStrings)
        {
            SaveDefault();
        }

        // 从嵌入资源加载语言文件
        Logger.Information("Loading language files...");
        var assembly = typeof(Strings).Assembly;
        foreach (var manifestResource in assembly.GetManifestResourceNames())
        {
            try
            {
                // 仅加载 Languages 目录下的 JSON 文件
                if (!manifestResource.StartsWith("SuchByte.MacroDeck.Resources.Languages.") ||
                    !manifestResource.EndsWith(".json"))
                {
                    continue;
                }

                Logger.Information("Loading ${ManifestResource}...", manifestResource);
                using var resourceStream = assembly.GetManifestResourceStream(manifestResource);

                var serializer = new JsonSerializer();
                using var sr = new StreamReader(resourceStream);
                using var jsonReader = new JsonTextReader(sr);
                while (!sr.EndOfStream)
                {
                    var language = serializer.Deserialize<Strings>(jsonReader);
                    // 去重：跳过已存在的同名、同语言代码、同作者的语言
                    if (_languages.FindAll(l =>
                            l.__Language__.Equals(language.__Language__) &&
                            l.__LanguageCode__.Equals(language.__LanguageCode__) &&
                            l.__Author__.Equals(language.__Author__)).Count >
                        0)
                    {
                        continue;
                    }

                    _languages.Add(language);
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Failed to load language resource");
            }
        }

        _languages = _languages.OrderBy(x => x.__Language__).ToList();
    }

    /// <summary>
    /// 将默认语言字符串导出为 JSON 文件到磁盘
    /// </summary>
    private static void SaveDefault()
    {
        var path = Path.Combine(ApplicationPaths.MainDirectoryPath, "Language", _strings.__Language__ + ".json");
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        try
        {
            using var sw = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, _strings);

            Logger.Information("{Language} saved", _strings.__Language__);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to save {Language}", _strings.__Language__);
        }
    }

    /// <summary>
    /// 根据语言名称设置当前语言
    /// </summary>
    /// <param name="languageName">语言名称</param>
    public static void SetLanguage(string languageName)
    {
        var strings = _languages.Find(l => l.__Language__.Equals(languageName));
        if (strings != null)
        {
            SetLanguage(strings);
        }
    }

    /// <summary>
    /// 设置当前语言并触发语言变更事件
    /// </summary>
    /// <param name="language">要设置的语言字符串实例</param>
    public static void SetLanguage(Strings language)
    {
        Logger.Information("Set language to {Language}", language.__Language__);
        _strings = language;
        LanguageChanged?.Invoke(language, EventArgs.Empty);
    }

    /// <summary>
    /// 获取当前语言名称
    /// </summary>
    public static string GetLanguageName()
    {
        return _strings.__Language__;
    }

    /// <summary>
    /// 获取当前语言代码
    /// </summary>
    public static string GetLanguageCode()
    {
        return _strings.__LanguageCode__;
    }
}
