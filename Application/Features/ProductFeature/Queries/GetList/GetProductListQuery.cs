using Application.Common.Pagination;
using Application.Features.ProductFeature.DTOs;
using MediatR;

namespace Application.Features.ProductFeature.Queries.GetList;

public sealed record GetProductListQuery(
    string? Search = null,
    bool? IsActive = null,
    DateTimeOffset? CreatedFrom = null,
    DateTimeOffset? CreatedTo = null,
    ProductSortField SortBy = ProductSortField.Name,
    SortingDirection SortDirection = SortingDirection.Ascending,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductDto>>;

public enum ProductSortField
{
    Name,
    IsActive,
    CreatedAt,
    UpdatedAt
}
