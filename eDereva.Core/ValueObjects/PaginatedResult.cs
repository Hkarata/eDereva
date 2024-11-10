namespace eDereva.Core.ValueObjects
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedResult(IEnumerable<T> items, int count, PaginationParams pagination)
        {
            Items = items;
            PageNumber = pagination.PageNumber;
            PageSize = pagination.PageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        }
    }
}
