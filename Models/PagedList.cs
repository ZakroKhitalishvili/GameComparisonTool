using System;

namespace GameComparisonTool.Models;

public class PagedList<T>
{
    public required IList<T> Items { get; set; }

    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}