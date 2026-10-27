using Application.Common.Pagination;
using Application.Features.CustomerFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.CustomerFeature.Queries.GetList;

public sealed record GetCustomerListQuery(
    string? Search = null,
    CustomerType? CustomerType = null,
    Guid? ProductId = null,
    bool? HasProduct = null,
    DateTimeOffset? CreatedFrom = null,
    DateTimeOffset? CreatedTo = null,
    CustomerSortField SortBy = CustomerSortField.FullName,
    SortingDirection SortDirection = SortingDirection.Ascending,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<CustomerDto>>;

public enum CustomerSortField
{
    FullName,
    CompanyName,
    CustomerType,
    CreatedAt,
    UpdatedAt
}
