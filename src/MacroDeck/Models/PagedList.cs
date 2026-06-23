namespace SuchByte.MacroDeck.Models;

/// <summary>
/// 分页列表泛型模型，用于扩展商店 API 的分页数据返回。
/// 包含当前页数据项、页码、每页大小和总数据量。
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public class PagedList<T>
{
    /// <summary>当前页的数据项列表</summary>
    public List<T>? Items { get; set; }

    /// <summary>当前页码（从 1 开始）</summary>
    public int Page { get; set; }

    /// <summary>每页数据项数量</summary>
    public int PageSize { get; set; }

    /// <summary>数据项总数</summary>
    public int TotalItems { get; set; }
}
