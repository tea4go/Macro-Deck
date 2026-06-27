using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;

namespace SuchByte.MacroDeck.Configuration;

/// <summary>
/// Macro Deck 主配置类，包含所有用户可配置的设置项。
/// 配置以 JSON 格式存储，支持自动启动、SSL、语言、更新等设置。
/// AutoStart 属性会自动管理 Windows 注册表中的启动项。
/// </summary>
public class MainConfiguration
{
    private static readonly ILogger Logger = Log.ForContext(typeof(MainConfiguration));

    /// <summary>
    /// 是否开机自动启动。设置时自动管理 Windows 注册表启动项。
    /// true 时添加注册表启动项，false 时删除。
    /// </summary>
    [JsonProperty("AutoStart")]
    public bool AutoStart
    {
        get;
        set
        {
            field = value;
            try
            {
                if (value)
                {
                    // 添加到 Windows 注册表启动项
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);
                    key?.SetValue("Macro Deck", Process.GetCurrentProcess().MainModule.FileName);
                }
                else
                {
                    // 从 Windows 注册表启动项中删除
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);
                    key?.DeleteValue("Macro Deck", false);
                }
            }
            catch
            {
                // 忽略注册表操作失败（可能权限不足）
            }
        }
    } = true;

    /// <summary>是否自动检查更新</summary>
    [JsonProperty("Update.Auto")] public bool AutoUpdates { get; set; } = true;

    /// <summary>是否更新到 Beta 版本</summary>
    [JsonProperty("Update.InstallBeta")] public bool UpdateBetaVersions { get; set; }

    /// <summary>是否启用 ADB 服务器（用于 Android 设备连接）</summary>
    [JsonProperty("Connection.Adb.Enabled")]
    public bool EnableAdbServer { get; set; } = true;

    /// <summary>是否在 ADB 连接时自动启动应用</summary>
    [JsonProperty("Connection.Adb.AutoStartApp")]
    public bool EnableAdbAutoStartApp { get; set; } = true;

    /// <summary>是否启用 SSL 加密连接</summary>
    [JsonProperty("Connection.Ssl.Enabled")]
    public bool EnableSsl { get; set; }

    /// <summary>SSL 证书 PEM 格式内容</summary>
    [JsonProperty("Connection.Ssl.Certificate.Pem")]
    public string? SslCertificatePem { get; set; }

    /// <summary>SSL 证书密钥（加密后的 PEM 格式）</summary>
    [JsonProperty("Connection.Ssl.Certificate.KeyEncrypted")]
    public string? SslCertificateKeyPemEncrypted { get; set; }

    /// <summary>服务器主机监听地址</summary>
    [JsonProperty("Connection.Host.Address")]
    public string HostAddress { get; set; } = "127.0.0.1";

    /// <summary>服务器主机监听端口</summary>
    [JsonProperty("Connection.Host.Port")] public int HostPort { get; set; } = 8191;

    /// <summary>新连接时是否弹出确认对话框</summary>
    [JsonProperty("Connection.AskOnNewConnections")]
    public bool AskOnNewConnections { get; set; } = true;

    /// <summary>是否阻止新设备连接</summary>
    [JsonProperty("Connection.BlockNewConnections")]
    public bool BlockNewConnections { get; set; }

    /// <summary>界面语言名称</summary>
    [JsonProperty("Language")] public string Language { get; set; } = "English";

    /// <summary>界面字体族名称，下次启动生效</summary>
    [JsonProperty("Font")] public string FontFamily { get; set; } = "Tahoma";

    /// <summary>界面基准字号，下次启动生效。各控件按相对基线(9.75)的修正量等比调整</summary>
    [JsonProperty("Font.Size")] public float FontSize { get; set; } = 9.75F;

    /// <summary>是否对所有界面文字叠加粗体，下次启动生效</summary>
    [JsonProperty("Font.Bold")] public bool FontBold { get; set; }

    /// <summary>是否发送匿名错误报告</summary>
    [JsonProperty("Privacy.SendAnonymousErrorReports")]
    public bool SendAnonymousErrorReports { get; set; } = true;

    /// <summary>
    /// 将当前配置保存到指定路径的 JSON 文件中
    /// </summary>
    /// <param name="path">配置文件保存路径</param>
    public void Save(string path)
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        try
        {
            using var sw = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, this);

            Logger.Information("配置已保存");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "保存配置失败");
        }
    }

    /// <summary>
    /// 从指定路径的 JSON 文件加载配置。如果文件内容无效则返回默认配置。
    /// </summary>
    /// <param name="path">配置文件路径</param>
    /// <returns>加载的配置实例或默认配置</returns>
    public static MainConfiguration LoadFromFile(string path)
    {
        return JsonConvert.DeserializeObject<MainConfiguration>(File.ReadAllText(path)) ?? new MainConfiguration();
    }
}
