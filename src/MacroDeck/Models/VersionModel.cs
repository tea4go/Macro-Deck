using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 版本号模型类，解析和表示 Macro Deck 的版本信息。
/// 支持正式版（如 "2.10.0"）和预览版（如 "2.10.0-preview1"）两种格式。
/// 在调试模式下会自动附加 "(debug)" 后缀标识。
/// </summary>
public class VersionModel
{
    /// <summary>预览版版本号正则表达式，格式：主版本.次版本.修订号-preview补丁号</summary>
    private const string PatternBeta = @"^(\d+)\.(\d+)\.(\d+)-preview(\d+)$";

    /// <summary>正式版版本号正则表达式，格式：主版本.次版本.修订号</summary>
    private const string PatternRelease = @"^(\d+)\.(\d+)\.(\d+)$";

    /// <summary>格式化后的版本字符串</summary>
    public string VersionString { get; }

    /// <summary>预览版补丁号（仅预览版有效）</summary>
    public int BetaPatch { get; }

    /// <summary>是否为预览版（Beta）</summary>
    public bool IsBeta { get; }

    /// <summary>构建号</summary>
    public int Build { get; set; }

    /// <summary>
    /// 构造函数，解析版本字符串并提取版本信息。
    /// </summary>
    /// <param name="versionString">版本字符串，如 "2.10.0" 或 "2.10.0-preview1"</param>
    /// <exception cref="ArgumentNullException">版本字符串为空时抛出</exception>
    public VersionModel(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
        {
            throw new ArgumentNullException();
        }

        var matchBeta = Regex.Match(versionString, PatternBeta);
        var matchRelease = Regex.Match(versionString, PatternRelease);

        if (matchBeta.Success)
        {
            IsBeta = true;
            if (int.TryParse(matchBeta.Groups[4].Value, out var betaPatch))
            {
                BetaPatch = betaPatch;
            }

            VersionString
                = $"{matchBeta.Groups[1].Value}.{matchBeta.Groups[2].Value}.{matchBeta.Groups[3].Value}-preview{BetaPatch}" +
                $"{(Debugger.IsAttached ? " (debug)" : "")}";
        }
        else if (matchRelease.Success)
        {
            VersionString = $"{matchRelease.Groups[1].Value}.{matchRelease.Groups[2].Value}.{matchRelease.Groups[3].Value} " +
                $"{(Debugger.IsAttached ? " (debug)" : "")}";
        }
        else
        {
            VersionString = "Unknown version";
        }
    }
}
