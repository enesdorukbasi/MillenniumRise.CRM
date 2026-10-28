using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.CustomerProductFeature.Queries.GetList;

public sealed class GetCustomerProductListQueryHandler(IRepository<CustomerProduct> customerProductRepository, IValidator<GetCustomerProductListQuery> validator, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<GetCustomerProductListQuery, PagedResult<CustomerProductDto>>
{
    public async Task<PagedResult<CustomerProductDto>> Handle(GetCustomerProductListQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        Expression<Func<CustomerProduct, bool>>? predicate = null;

        if (request.Search.IsNotNullOrEmpty())
        {
            predicate = predicate.And(customerProduct =>
                customerProduct.Customer.FullName.Contains(request.Search!) ||
                (customerProduct.Customer.CompanyName != null && customerProduct.Customer.CompanyName.Contains(request.Search!)) ||
                customerProduct.Product.Name.Contains(request.Search!) ||
                (customerProduct.StatusNote != null && customerProduct.StatusNote.Contains(request.Search!)));
        }

        if (request.CustomerId.HasValue)
            predicate = predicate.And(customerProduct => customerProduct.CustomerId == request.CustomerId.Value);

        if (request.ProductId.HasValue)
            predicate = predicate.And(customerProduct => customerProduct.ProductId == request.ProductId.Value);

        if (request.Status.HasValue)
            predicate = predicate.And(customerProduct => customerProduct.Status == request.Status.Value);

        if (request.NextFollowUpFrom.HasValue)
            predicate = predicate.And(customerProduct => customerProduct.NextFollowUpAt >= request.NextFollowUpFrom.Value);

        if (request.NextFollowUpTo.HasValue)
            predicate = predicate.And(customerProduct => customerProduct.NextFollowUpAt <= request.NextFollowUpTo.Value);

        Func<IQueryable<CustomerProduct>, IOrderedQueryable<CustomerProduct>> orderBy = (request.SortBy, request.SortDirection) switch
        {
            (CustomerProductSortField.CustomerName, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.Customer.FullName).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.ProductName, SortingDirection.Ascending) => query => query.OrderBy(customerProduct => customerProduct.Product.Name).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.ProductName, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.Product.Name).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.Status, SortingDirection.Ascending) => query => query.OrderBy(customerProduct => customerProduct.Status).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.Status, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.Status).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.NextFollowUpAt, SortingDirection.Ascending) => query => query.OrderBy(customerProduct => customerProduct.NextFollowUpAt).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.NextFollowUpAt, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.NextFollowUpAt).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.CreatedAt, SortingDirection.Ascending) => query => query.OrderBy(customerProduct => customerProduct.CreatedAt).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.CreatedAt, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.CreatedAt).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.UpdatedAt, SortingDirection.Ascending) => query => query.OrderBy(customerProduct => customerProduct.UpdatedAt).ThenBy(customerProduct => customerProduct.Id),
            (CustomerProductSortField.UpdatedAt, SortingDirection.Descending) => query => query.OrderByDescending(customerProduct => customerProduct.UpdatedAt).ThenBy(customerProduct => customerProduct.Id),
            _ => query => query.OrderBy(customerProduct => customerProduct.Customer.FullName).ThenBy(customerProduct => customerProduct.Id)
        };

        var totalCount = await customerProductRepository.GetCount(predicate);
        var customerProducts = await customerProductRepository.GetAllAsync(
            filter: predicate,
            orderBy: orderBy,
            include: query => query
                .Include(customerProduct => customerProduct.Customer)
                .Include(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo)
                .Include(customerProduct => customerProduct.Interactions.OrderByDescending(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt).Take(1)),
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            asNoTracking: true,
            splitQuery: false);

        return new PagedResult<CustomerProductDto>(
            customerProductMapper.Map(customerProducts.ToList()),
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
