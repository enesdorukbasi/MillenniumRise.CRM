using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.DashboardFeature.Queries.GetRecentInteractions;

public sealed class GetRecentInteractionsQueryHandler(IRepository<Interaction> interactionRepository, IValidator<GetRecentInteractionsQuery> validator, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<GetRecentInteractionsQuery, PagedResult<InteractionDto>>
{
    public async Task<PagedResult<InteractionDto>> Handle(GetRecentInteractionsQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var totalCount = await interactionRepository.GetCount();
        var interactions = await interactionRepository.GetAllAsync(
            orderBy: query => query
                .OrderByDescending(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt)
                .ThenBy(interaction => interaction.Id),
            include: query => query
                .Include(interaction => interaction.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Customer)
                .Include(interaction => interaction.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo),
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            asNoTracking: true,
            splitQuery: false);

        return new PagedResult<InteractionDto>(
            interactionMapper.Map(interactions.ToList()),
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
