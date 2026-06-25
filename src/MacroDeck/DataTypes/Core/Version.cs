using System.Text.RegularExpressions;

namespace SuchByte.MacroDeck.DataTypes.Core;

public partial struct Version
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    /// <summary>Beta 版本号，使用 long 类型以支持时间戳等大数值构建号。</summary>
    public long? BetaNo { get; set; }

    public Version(int major, int minor, int patch, long? betaNo = null)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        BetaNo = betaNo;
    }

    public bool IsBetaVersion => BetaNo.HasValue;

    public string VersionName => BetaNo.HasValue
        ? $"{Major}.{Minor}.{Patch}-b{BetaNo}"
        : $"{Major}.{Minor}.{Patch}";

    public override string ToString()
    {
        return VersionName;
    }

    public static bool TryParse(string versionString, out Version result)
    {
        try
        {
            result = Parse(versionString);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public static Version Parse(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
        {
            throw new FormatException("Version string was empty");
        }

        var match = VersionRegex().Match(versionString);
        if (!match.Success)
        {
            throw new FormatException("Invalid version string");
        }

        var major = int.Parse(match.Groups["major"].Value);
        var minor = int.Parse(match.Groups["minor"].Value);
        var patch = int.Parse(match.Groups["patch"].Value);

        long? previewNo = null;
        if (match.Groups["beta"].Success)
        {
            previewNo = long.Parse(match.Groups["beta"].Value);
        }

        return new Version(major, minor, patch, previewNo);
    }

    [GeneratedRegex("^(?<major>\\d+)\\.(?<minor>\\d+)\\.(?<patch>\\d+)(-b(?<beta>\\d+))?$")]
    private static partial Regex VersionRegex();
}
