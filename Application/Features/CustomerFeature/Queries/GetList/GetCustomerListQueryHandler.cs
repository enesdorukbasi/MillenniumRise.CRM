using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.CustomerFeature.Queries.GetList;

public sealed class GetCustomerListQueryHandler(IRepository<Customer> customerRepository, IValidator<GetCustomerListQuery> validator, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerDto>>
{
    public async Task<PagedResult<CustomerDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        Expression<Func<Customer, bool>>? predicate = null;

        if (request.Search.IsNotNullOrEmpty())
        {
            predicate = predicate.And(customer =>
                customer.FullName.Contains(request.Search!) ||
                (customer.CompanyName != null && customer.CompanyName.Contains(request.Search!)) ||
                (customer.PhoneNumber != null && customer.PhoneNumber.Contains(request.Search!)) ||
                (customer.Email != null && customer.Email.Contains(request.Search!)) ||
                (customer.Sector != null && customer.Sector.Contains(request.Search!)) ||
                (customer.Location != null && customer.Location.Contains(request.Search!)));
        }

        if (request.CustomerType.HasValue)
            predicate = predicate.And(customer => customer.CustomerType == request.CustomerType.Value);

        if (request.ProductId.HasValue && request.HasProduct.HasValue)
        {
            Expression<Func<Customer, bool>> productPredicate = customer => customer.CustomerProducts.Any(customerProduct => customerProduct.ProductId == request.ProductId.Value);
            predicate = request.HasProduct.Value
                ? predicate.And(productPredicate)
                : predicate.And(productPredicate.Not());
        }

        if (request.CreatedFrom.HasValue)
            predicate = predicate.And(customer => customer.CreatedAt >= request.CreatedFrom.Value);

        if (request.CreatedTo.HasValue)
            predicate = predicate.And(customer => customer.CreatedAt <= request.CreatedTo.Value);

        Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = (request.SortBy, request.SortDirection) switch
        {
            (CustomerSortField.FullName, SortingDirection.Descending) => query => query.OrderByDescending(customer => customer.FullName).ThenBy(customer => customer.Id),
            (CustomerSortField.CompanyName, SortingDirection.Ascending) => query => query.OrderBy(customer => customer.CompanyName).ThenBy(customer => customer.Id),
            (CustomerSortField.CompanyName, SortingDirection.Descending) => query => query.OrderByDescending(customer => customer.CompanyName).ThenBy(customer => customer.Id),
            (CustomerSortField.CustomerType, SortingDirection.Ascending) => query => query.OrderBy(customer => customer.CustomerType).ThenBy(customer => customer.Id),
            (CustomerSortField.CustomerType, SortingDirection.Descending) => query => query.OrderByDescending(customer => customer.CustomerType).ThenBy(customer => customer.Id),
            (CustomerSortField.CreatedAt, SortingDirection.Ascending) => query => query.OrderBy(customer => customer.CreatedAt).ThenBy(customer => customer.Id),
            (CustomerSortField.CreatedAt, SortingDirection.Descending) => query => query.OrderByDescending(customer => customer.CreatedAt).ThenBy(customer => customer.Id),
            (CustomerSortField.UpdatedAt, SortingDirection.Ascending) => query => query.OrderBy(customer => customer.UpdatedAt).ThenBy(customer => customer.Id),
            (CustomerSortField.UpdatedAt, SortingDirection.Descending) => query => query.OrderByDescending(customer => customer.UpdatedAt).ThenBy(customer => customer.Id),
            _ => query => query.OrderBy(customer => customer.FullName).ThenBy(customer => customer.Id)
        };

        var totalCount = await customerRepository.GetCount(predicate);
        var customers = await customerRepository.GetAllAsync(
            filter: predicate,
            orderBy: orderBy,
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            asNoTracking: true,
            splitQuery: false);

        return new PagedResult<CustomerDto>(
            customerMapper.Map(customers.ToList()),
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
