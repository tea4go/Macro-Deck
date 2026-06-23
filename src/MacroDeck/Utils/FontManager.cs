using System.Drawing.Text;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Serilog;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 全局字体管理器。在启动时根据配置初始化界面字体族，并提供递归替换
/// 控件树字体的能力。设计目标：在不改动大量 Designer 硬编码字体的前提下，
/// 让所有界面统一使用用户选择的字体族，同时保留各控件原有的字号与样式。
/// 支持运行时实时刷新：缓存每个控件的原始字体，重复 Apply 始终基于原始字体
/// 重算，因而幂等、可反复调用（包括把字号改小）。
/// </summary>
public static class FontManager
{
    private static readonly ILogger Logger = Log.ForContext(typeof(FontManager));

    /// <summary>缓存每个控件首次被处理时的原始字体，用于幂等重算（实时刷新）。</summary>
    private static readonly ConditionalWeakTable<Control, Font> OriginalFonts = new();

    private const string DefaultFontFamily = "Tahoma";

    /// <summary>
    /// 基准字号（基线）。界面中出现最多的字号，用于计算各控件相对基线的修正量：
    /// 修正量 = 控件原字号 - 基线；应用后字号 = 用户设定字号 + 修正量。
    /// 这样统一调整字号时仍保留各控件原有的相对大小层次。
    /// </summary>
    private const float BaselineFontSize = 9.75F;

    /// <summary>当前生效的界面字体族名称，启动时初始化一次。</summary>
    public static string FontFamily { get; private set; } = DefaultFontFamily;

    /// <summary>当前生效的基准字号（用户设定值），启动时初始化一次。</summary>
    public static float FontSize { get; private set; } = BaselineFontSize;

    /// <summary>是否对所有控件叠加粗体，启动时初始化一次。</summary>
    public static bool FontBold { get; private set; }

    /// <summary>
    /// 根据配置初始化字体族、字号与粗体。若配置的字体未安装，则回退到默认字体并记录日志。
    /// 缓存配置供 Apply/Resolve 使用。
    /// 必须在创建第一个窗体之前调用（兜底的 SetDefaultFont 请用 SetDefaultFontEarly 在
    /// 程序入口更早调用，因为它要求早于任何 Win32 窗口的创建）。
    /// </summary>
    /// <param name="configuredFamily">配置中的字体族名称</param>
    /// <param name="configuredSize">配置中的基准字号</param>
    /// <param name="configuredBold">配置中是否粗体</param>
    public static void Initialize(string? configuredFamily, float configuredSize, bool configuredBold)
    {
        var family = string.IsNullOrWhiteSpace(configuredFamily) ? DefaultFontFamily : configuredFamily.Trim();

        if (!string.Equals(family, DefaultFontFamily, StringComparison.OrdinalIgnoreCase) && !IsFontInstalled(family))
        {
            Logger.Warning("Configured font \"{Font}\" is not installed, falling back to {Default}", family,
                DefaultFontFamily);
            family = DefaultFontFamily;
        }

        FontFamily = family;
        FontSize = configuredSize > 0 ? configuredSize : BaselineFontSize;
        FontBold = configuredBold;
    }

    /// <summary>
    /// 更新当前生效的字体配置并立即刷新所有已打开的窗体（实时生效）。
    /// 字号采用基于各控件原始字体重算的方式，因而支持反复调整、改小、改回默认。
    /// 注意：非控件树的自定义绘制（卡片、托盘菜单等）不在此实时刷新范围内，需重启生效。
    /// </summary>
    /// <param name="configuredFamily">字体族名称</param>
    /// <param name="configuredSize">基准字号</param>
    /// <param name="configuredBold">是否粗体</param>
    public static void UpdateAndRefresh(string? configuredFamily, float configuredSize, bool configuredBold)
    {
        Initialize(configuredFamily, configuredSize, configuredBold);

        foreach (System.Windows.Forms.Form form in Application.OpenForms)
        {
            try
            {
                Apply(form);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Failed to refresh font for form {Form}", form.Name);
            }
        }
    }

    /// <summary>
    /// 判断当前配置是否为默认值（字体 Tahoma、字号等于基线、未加粗）。
    /// 为默认时无需改动任何控件，直接短路以保证零开销零回归。
    /// </summary>
    private static bool IsDefault()
    {
        return string.Equals(FontFamily, DefaultFontFamily, StringComparison.OrdinalIgnoreCase)
            && Math.Abs(FontSize - BaselineFontSize) < 0.01F
            && !FontBold;
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
            if (!File.Exists(configFilePath))
            {
                return;
            }

            var json = JObject.Parse(File.ReadAllText(configFilePath));
            var family = json["Font"]?.ToString();
            var size = (float?)json["Font.Size"] ?? BaselineFontSize;
            var bold = (bool?)json["Font.Bold"] ?? false;

            var resolvedFamily = string.IsNullOrWhiteSpace(family) ? DefaultFontFamily : family.Trim();
            if (!string.Equals(resolvedFamily, DefaultFontFamily, StringComparison.OrdinalIgnoreCase)
                && !IsFontInstalled(resolvedFamily))
            {
                resolvedFamily = DefaultFontFamily;
            }

            // 配置全为默认值时无需设置默认字体
            if (string.Equals(resolvedFamily, DefaultFontFamily, StringComparison.OrdinalIgnoreCase)
                && Math.Abs(size - BaselineFontSize) < 0.01F && !bold)
            {
                return;
            }

            var style = bold ? FontStyle.Bold : FontStyle.Regular;
            Application.SetDefaultFont(new Font(resolvedFamily, size > 0 ? size : BaselineFontSize, style));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to set application default font");
        }
    }

    /// <summary>
    /// 递归地将控件及其所有子控件的字体替换为配置字体。字号按"用户设定字号 + 控件相对
    /// 基线的修正量"计算以保留层次，粗体按配置叠加，字体族换为配置族。
    /// 始终基于控件的原始字体重算（首次处理时缓存），因而可反复调用、幂等，支持运行时
    /// 实时刷新（包括把字号改小、或改回默认时还原成原始字体）。
    /// 控件的 Tag 为 "no-font" 时跳过（用于等宽对齐等不应更改字体的控件）。
    /// </summary>
    /// <param name="root">要处理的根控件（通常是窗体自身）</param>
    public static void Apply(Control root)
    {
        ApplyRecursive(root);
    }

    private static void ApplyRecursive(Control control)
    {
        if (control.Tag as string != "no-font")
        {
            try
            {
                // 缓存首次处理时的原始字体，后续始终基于它重算（保证幂等、可还原）
                if (!OriginalFonts.TryGetValue(control, out var original))
                {
                    original = control.Font;
                    OriginalFonts.Add(control, original);
                }

                var target = IsDefault() ? original : BuildFont(original);
                if (!control.Font.Equals(target))
                {
                    control.Font = target;
                }
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
    /// 根据原字体返回应用配置后的新字体，供自定义绘制等不在控件树中的场景调用。
    /// 配置为默认时直接返回原字体。
    /// </summary>
    /// <param name="original">原字体</param>
    /// <returns>应用配置后的字体</returns>
    public static Font Resolve(Font original)
    {
        if (IsDefault())
        {
            return original;
        }

        return BuildFont(original);
    }

    /// <summary>
    /// 基于原字体构造应用了配置字体族、字号修正和粗体叠加的新字体，保留原单位与字符集。
    /// </summary>
    private static Font BuildFont(Font original)
    {
        // 字号 = 用户设定字号 + 控件原字号相对基线的修正量，保留视觉层次
        var size = FontSize + (original.Size - BaselineFontSize);
        if (size <= 0)
        {
            size = original.Size;
        }

        var style = FontBold ? original.Style | FontStyle.Bold : original.Style;

        return new Font(FontFamily, size, style, original.Unit, original.GdiCharSet);
    }

    private static bool IsFontInstalled(string family)
    {
        using var collection = new InstalledFontCollection();
        return collection.Families.Any(f => string.Equals(f.Name, family, StringComparison.OrdinalIgnoreCase));
    }
}
