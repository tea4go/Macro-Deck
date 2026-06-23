namespace SuchByte.MacroDeck.Utils;

/// <summary>
/// 字符串搜索工具类，提供忽略大小写和空格的字符串包含判断
/// </summary>
public class StringSearch
{
    /// <summary>
    /// 判断字符串是否包含搜索文本（忽略大小写和空格）
    /// </summary>
    /// <param name="str">目标字符串</param>
    /// <param name="search">搜索文本</param>
    /// <returns>包含则返回 true，否则返回 false</returns>
    public static bool StringContains(string str, string search)
    {
        return str.ToLower().Replace(" ", "").Contains(search.ToLower().Replace(" ", ""));
    }
}
