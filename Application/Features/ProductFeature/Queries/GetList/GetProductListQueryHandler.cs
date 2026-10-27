using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.ProductFeature.Queries.GetList;

public sealed class GetProductListQueryHandler(IRepository<Product> productRepository, IValidator<GetProductListQuery> validator, IMapper<Product, ProductDto> productMapper) : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        Expression<Func<Product, bool>>? predicate = null;

        if (request.Search.IsNotNullOrEmpty())
        {
            predicate = predicate.And(product =>
                product.Name.Contains(request.Search!) ||
                (product.Description != null && product.Description.Contains(request.Search!)));
        }

        if (request.IsActive.HasValue)
            predicate = predicate.And(product => product.IsActive == request.IsActive.Value);

        if (request.CreatedFrom.HasValue)
            predicate = predicate.And(product => product.CreatedAt >= request.CreatedFrom.Value);

        if (request.CreatedTo.HasValue)
            predicate = predicate.And(product => product.CreatedAt <= request.CreatedTo.Value);

        Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = (request.SortBy, request.SortDirection) switch
        {
            (ProductSortField.Name, SortingDirection.Descending) => query => query.OrderByDescending(product => product.Name).ThenBy(product => product.Id),
            (ProductSortField.IsActive, SortingDirection.Ascending) => query => query.OrderBy(product => product.IsActive).ThenBy(product => product.Id),
            (ProductSortField.IsActive, SortingDirection.Descending) => query => query.OrderByDescending(product => product.IsActive).ThenBy(product => product.Id),
            (ProductSortField.CreatedAt, SortingDirection.Ascending) => query => query.OrderBy(product => product.CreatedAt).ThenBy(product => product.Id),
            (ProductSortField.CreatedAt, SortingDirection.Descending) => query => query.OrderByDescending(product => product.CreatedAt).ThenBy(product => product.Id),
            (ProductSortField.UpdatedAt, SortingDirection.Ascending) => query => query.OrderBy(product => product.UpdatedAt).ThenBy(product => product.Id),
            (ProductSortField.UpdatedAt, SortingDirection.Descending) => query => query.OrderByDescending(product => product.UpdatedAt).ThenBy(product => product.Id),
            _ => query => query.OrderBy(product => product.Name).ThenBy(product => product.Id)
        };

        var totalCount = await productRepository.GetCount(predicate);
        var products = await productRepository.GetAllAsync(
            filter: predicate,
            orderBy: orderBy,
            include: query => query.Include(product => product.ProductLogo),
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            asNoTracking: true,
            splitQuery: false);

        return new PagedResult<ProductDto>(
            productMapper.Map(products.ToList()),
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
