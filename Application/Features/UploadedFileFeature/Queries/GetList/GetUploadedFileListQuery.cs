using Application.Common.Pagination;
using Application.Features.UploadedFileFeature.DTOs;
using MediatR;

namespace Application.Features.UploadedFileFeature.Queries.GetList;

public sealed record GetUploadedFileListQuery(
    string? Search = null,
    string? Extension = null,
    string? ContentType = null,
    long? MinimumFileSize = null,
    long? MaximumFileSize = null,
    DateTimeOffset? CreatedFrom = null,
    DateTimeOffset? CreatedTo = null,
    UploadedFileSortField SortBy = UploadedFileSortField.CreatedAt,
    SortingDirection SortDirection = SortingDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<UploadedFileDto>>;

public enum UploadedFileSortField
{
    Name,
    OriginalFileName,
    Extension,
    ContentType,
    FileSize,
    CreatedAt,
    UpdatedAt
}
