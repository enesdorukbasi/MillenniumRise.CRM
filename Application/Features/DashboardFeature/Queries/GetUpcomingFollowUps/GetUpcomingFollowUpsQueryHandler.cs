using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.DashboardFeature.Queries.GetUpcomingFollowUps;

public sealed class GetUpcomingFollowUpsQueryHandler(IRepository<CustomerProduct> customerProductRepository, IValidator<GetUpcomingFollowUpsQuery> validator, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<GetUpcomingFollowUpsQuery, PagedResult<CustomerProductDto>>
{
    public async Task<PagedResult<CustomerProductDto>> Handle(GetUpcomingFollowUpsQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var startDate = request.StartDate ?? DateTimeOffset.UtcNow;
        var endDate = request.EndDate ?? startDate.AddDays(7);

        Expression<Func<CustomerProduct, bool>>? predicate = null;
        predicate = predicate.And(customerProduct =>
            customerProduct.NextFollowUpAt.HasValue &&
            customerProduct.NextFollowUpAt.Value >= startDate &&
            customerProduct.NextFollowUpAt.Value <= endDate &&
            customerProduct.Status != CustomerProductStatus.Successful &&
            customerProduct.Status != CustomerProductStatus.Unsuccessful);

        var totalCount = await customerProductRepository.GetCount(predicate);
        var customerProducts = await customerProductRepository.GetAllAsync(
            filter: predicate,
            orderBy: query => query
                .OrderBy(customerProduct => customerProduct.NextFollowUpAt)
                .ThenBy(customerProduct => customerProduct.Id),
            include: query => query
                .Include(customerProduct => customerProduct.Customer)
                .Include(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo),
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
