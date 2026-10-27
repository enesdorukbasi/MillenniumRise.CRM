namespace Application.Common.Pagination;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, long TotalCount)
{
    public long TotalPages => TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
