namespace Once.Domain.Abstractions;

public class PagedList<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
    {
        Items = items.ToList();
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public List<T> Items { get; set; } // Data for the current page
    public int TotalCount { get; set; } // Total number of items
    public int PageIndex { get; set; } // Current page number
    public int PageSize { get; set; } // Number of items per page
    public int TotalPages { get; set; } // Total number of pages
}