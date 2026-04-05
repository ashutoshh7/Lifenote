namespace Lifenote.API.Models.Responses;

/// <summary>
/// Paginated API response envelope.
/// Wraps a page of items with total count metadata.
/// </summary>
public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public static PaginatedResponse<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize) =>
        new() { Items = items.ToList().AsReadOnly(), TotalCount = totalCount, Page = page, PageSize = pageSize };
}
