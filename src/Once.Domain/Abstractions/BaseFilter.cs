namespace Once.Domain.Abstractions;

public record BaseFilter
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            if (value < 1)
            {
                _pageIndex = 1;
            }
            _pageIndex = value;
        }
    }

    /// <summary>
    /// Resource paging: PageSize count
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0 || value > 100)
                _pageSize = 10;
            else
                _pageSize = value;
        }
    }

    public int Skip => (PageIndex - 1) * PageSize;
}
