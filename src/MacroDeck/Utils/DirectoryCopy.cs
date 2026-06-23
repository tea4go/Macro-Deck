using System.IO;

namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 目录复制工具类，递归复制源目录的所有文件和子目录到目标目录。
/// 用于插件更新时将 .updates 目录中的文件复制到插件目录。
/// </summary>
public static class DirectoryCopy
{
    /// <summary>
    /// 递归复制目录。
    /// 将源目录中的所有文件复制到目标目录，可选是否复制子目录。
    /// 如果目标目录不存在则自动创建。
    /// </summary>
    /// <param name="sourceDirName">源目录路径</param>
    /// <param name="destDirName">目标目录路径</param>
    /// <param name="copySubDirs">是否复制子目录</param>
    /// <exception cref="DirectoryNotFoundException">源目录不存在时抛出</exception>
    public static void Copy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        var dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " +
                sourceDirName);
        }

        var dirs = dir.GetDirectories();

        // 如果目标目录不存在，则创建
        Directory.CreateDirectory(destDirName);

        // 复制当前目录中的所有文件
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            var tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, true);
        }

        // 递归复制子目录
        if (copySubDirs)
        {
            foreach (var subdir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subdir.Name);
                Copy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}
