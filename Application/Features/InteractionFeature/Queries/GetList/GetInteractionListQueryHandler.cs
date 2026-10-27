using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.InteractionFeature.Queries.GetList;

public sealed class GetInteractionListQueryHandler(IRepository<Interaction> interactionRepository, IValidator<GetInteractionListQuery> validator, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<GetInteractionListQuery, List<InteractionDto>>
{
    public async Task<List<InteractionDto>> Handle(GetInteractionListQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        Expression<Func<Interaction, bool>>? predicate = null;

        if (request.CustomerProductId.HasValue)
            predicate = predicate.And(interaction => interaction.CustomerProductId == request.CustomerProductId.Value);

        if (request.Status.HasValue)
            predicate = predicate.And(interaction => interaction.Status == request.Status.Value);

        if (request.Channel.HasValue)
            predicate = predicate.And(interaction => interaction.Channel == request.Channel.Value);

        if (request.StartDate.HasValue)
            predicate = predicate.And(interaction => (interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt) >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            predicate = predicate.And(interaction => (interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt) <= request.EndDate.Value);

        var interactions = await interactionRepository.GetAllAsync(
            filter: predicate,
            orderBy: query => query.OrderByDescending(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt),
            include: query => query
                .Include(interaction => interaction.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Customer)
                .Include(interaction => interaction.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo),
            asNoTracking: true,
            splitQuery: false);

        return interactionMapper.Map(interactions.ToList());
    }
}
