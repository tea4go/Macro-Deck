namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 操作系统信息工具类，提供 Windows 版本名称获取功能。
/// 根据系统版本号映射为对应的 Windows 版本名称（如 Windows 10、Windows 11）。
/// </summary>
public static class OperatingSystemInformation
{
    /// <summary>
    /// 获取当前 Windows 版本的友好名称。
    /// 通过 Environment.OSVersion 的主版本号和次版本号映射版本名称，
    /// 并附加构建号和位数信息。
    /// </summary>
    /// <returns>格式如 "Windows 11 build 22631 64 bit" 的版本字符串</returns>
    public static string GetWindowsVersionName()
    {
        var os = Environment.OSVersion;
        var version = os.Version;

        var versionString = "";

        switch (version.Major)
        {
            case 6:
                switch (version.Minor)
                {
                    case 0:
                        versionString = "Vista";
                        break;
                    case 1:
                        versionString = "7";
                        break;
                    case 2:
                        versionString = "8";
                        break;
                    case 3:
                        versionString = "8.1";
                        break;
                    default:
                        versionString = "unknown";
                        break;
                }

                break;
            case 10:
                // 构建号 22000 及以上为 Windows 11
                if (os.Version.Build < 22000)
                {
                    versionString = "10";
                }
                else
                {
                    versionString = "11";
                }

                break;
        }

        return "Windows " +
            versionString +
            " build " +
            os.Version.Build +
            (Environment.Is64BitOperatingSystem ? " 64 bit" : " 32 bit");
    }
}
