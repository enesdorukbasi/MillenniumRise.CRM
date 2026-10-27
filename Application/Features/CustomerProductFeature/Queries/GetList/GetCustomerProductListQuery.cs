using Application.Common.Pagination;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.CustomerProductFeature.Queries.GetList;

public sealed record GetCustomerProductListQuery(
    string? Search = null,
    Guid? CustomerId = null,
    Guid? ProductId = null,
    CustomerProductStatus? Status = null,
    DateTimeOffset? NextFollowUpFrom = null,
    DateTimeOffset? NextFollowUpTo = null,
    CustomerProductSortField SortBy = CustomerProductSortField.CustomerName,
    SortingDirection SortDirection = SortingDirection.Ascending,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<CustomerProductDto>>;

public enum CustomerProductSortField
{
    CustomerName,
    ProductName,
    Status,
    NextFollowUpAt,
    CreatedAt,
    UpdatedAt
}
