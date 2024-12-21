namespace eDereva.Core.ValueObjects;

public class PaginatedResult<T>
{
    // Parameterless constructor for deserialization
    public PaginatedResult()
    {
    }

    // Constructor to set values during instantiation
    public PaginatedResult(IEnumerable<T> items, int count, PaginationParams pagination)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        PageNumber = pagination.PageNumber;
        PageSize = pagination.PageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pagination.PageSize);
    }

    public IEnumerable<T> Items { get; set; } = null!; // Added setter for deserialization
    public int PageNumber { get; set; } // Added setter for deserialization
    public int PageSize { get; set; } // Added setter for deserialization
    public int TotalPages { get; set; } // Added setter for deserialization
    public int TotalCount { get; set; } // Added setter for deserialization
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}