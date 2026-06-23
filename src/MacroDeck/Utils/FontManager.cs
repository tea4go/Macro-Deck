using System.Drawing.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Serilog;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 全局字体管理器。在启动时根据配置初始化界面字体族，并提供递归替换
/// 控件树字体的能力。设计目标：在不改动大量 Designer 硬编码字体的前提下，
/// 让所有界面统一使用用户选择的字体族，同时保留各控件原有的字号与样式。
/// </summary>
public static class FontManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(FontManager));

    private const string DefaultFontFamily = "Tahoma";

    /// <summary>当前生效的界面字体族名称，启动时初始化一次。</summary>
    public static string FontFamily { get; private set; } = DefaultFontFamily;

    /// <summary>
    /// 根据配置初始化字体族。若配置的字体未安装，则回退到默认字体并记录日志。
    /// 缓存 FontFamily 供 Apply/Resolve 使用。
    /// 必须在创建第一个窗体之前调用（兜底的 SetDefaultFont 请用 SetDefaultFontEarly 在
    /// 程序入口更早调用，因为它要求早于任何 Win32 窗口的创建）。
    /// </summary>
    /// <param name="configuredFamily">配置中的字体族名称</param>
    public static void Initialize(string? configuredFamily)
    {
        var family = string.IsNullOrWhiteSpace(configuredFamily) ? DefaultFontFamily : configuredFamily.Trim();

        if (!string.Equals(family, DefaultFontFamily, StringComparison.OrdinalIgnoreCase) && !IsFontInstalled(family))
        {
            Logger.Warning("Configured font \"{Font}\" is not installed, falling back to {Default}", family,
                DefaultFontFamily);
            family = DefaultFontFamily;
        }

        FontFamily = family;
    }

    /// <summary>
    /// 在程序入口最早期（任何 Win32 窗口创建之前）设置 WinForms 应用程序默认字体，
    /// 作为兜底，覆盖未显式设置字体的控件。直接读取配置文件而不依赖已加载的全局配置。
    /// </summary>
    /// <param name="configFilePath">主配置文件路径</param>
    public static void SetDefaultFontEarly(string configFilePath)
    {
        try
        {
            var family = ReadFontFamilyFromConfig(configFilePath);
            if (string.IsNullOrWhiteSpace(family) ||
                string.Equals(family, DefaultFontFamily, StringComparison.OrdinalIgnoreCase) ||
                !IsFontInstalled(family))
            {
                return;
            }

            Application.SetDefaultFont(new Font(family, 9F));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to set application default font");
        }
    }

    private static string? ReadFontFamilyFromConfig(string configFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            return null;
        }

        var json = JObject.Parse(File.ReadAllText(configFilePath));
        return json["Font"]?.ToString();
    }

    /// <summary>
    /// 递归地将控件及其所有子控件的字体替换为配置字体族，保留原有字号、样式和单位。
    /// 当用户未更改字体（仍为默认 Tahoma）时直接返回，零开销。
    /// 控件的 Tag 为 "no-font" 时跳过（用于等宽对齐等不应更改字体的控件）。
    /// </summary>
    /// <param name="root">要处理的根控件（通常是窗体自身）</param>
    public static void Apply(Control root)
    {
        if (string.Equals(FontFamily, DefaultFontFamily, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ApplyRecursive(root);
    }

    private static void ApplyRecursive(Control control)
    {
        if (control.Tag as string != "no-font")
        {
            try
            {
                var old = control.Font;
                control.Font = new Font(FontFamily, old.Size, old.Style, old.Unit, old.GdiCharSet);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Failed to apply font to control {Control}", control.Name);
            }
        }

        foreach (Control child in control.Controls)
        {
            ApplyRecursive(child);
        }
    }

    /// <summary>
    /// 根据原字体返回替换字体族后的新字体（保留字号/样式/单位），供自定义绘制等
    /// 不在控件树中的场景调用。未更改字体时直接返回原字体。
    /// </summary>
    /// <param name="original">原字体</param>
    /// <returns>替换字体族后的字体</returns>
    public static Font Resolve(Font original)
    {
        if (string.Equals(FontFamily, DefaultFontFamily, StringComparison.OrdinalIgnoreCase))
        {
            return original;
        }

        return new Font(FontFamily, original.Size, original.Style, original.Unit, original.GdiCharSet);
    }

    private static bool IsFontInstalled(string family)
    {
        using var collection = new InstalledFontCollection();
        return collection.Families.Any(f => string.Equals(f.Name, family, StringComparison.OrdinalIgnoreCase));
    }
}
