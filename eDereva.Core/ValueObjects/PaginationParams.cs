namespace eDereva.Core.ValueObjects;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    // Default constructor for deserialization
    public PaginationParams()
    {
    }

    // Constructor to easily set values when needed
    public PaginationParams(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isDescending = false)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortBy = sortBy;
        IsDescending = isDescending;
    }

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }

    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}