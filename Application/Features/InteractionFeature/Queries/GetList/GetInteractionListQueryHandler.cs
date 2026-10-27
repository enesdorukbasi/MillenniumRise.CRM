using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Features.InteractionFeature.Queries.GetList;

public sealed class GetInteractionListQueryHandler(IRepository<Interaction> interactionRepository, IValidator<GetInteractionListQuery> validator, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<GetInteractionListQuery, PagedResult<InteractionDto>>
{
    public async Task<PagedResult<InteractionDto>> Handle(GetInteractionListQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        Expression<Func<Interaction, bool>>? predicate = null;

        if (request.Search.IsNotNullOrEmpty())
        {
            predicate = predicate.And(interaction =>
                interaction.Notes.Contains(request.Search!) ||
                interaction.CustomerProduct.Customer.FullName.Contains(request.Search!) ||
                (interaction.CustomerProduct.Customer.CompanyName != null && interaction.CustomerProduct.Customer.CompanyName.Contains(request.Search!)) ||
                interaction.CustomerProduct.Product.Name.Contains(request.Search!));
        }

        if (request.CustomerProductId.HasValue)
            predicate = predicate.And(interaction => interaction.CustomerProductId == request.CustomerProductId.Value);

        if (request.Status.HasValue)
            predicate = predicate.And(interaction => interaction.Status == request.Status.Value);

        if (request.Channel.HasValue)
            predicate = predicate.And(interaction => interaction.Channel == request.Channel.Value);

        if (request.Outcome.HasValue)
            predicate = predicate.And(interaction => interaction.Outcome == request.Outcome.Value);

        if (request.StartDate.HasValue)
            predicate = predicate.And(interaction => (interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt) >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            predicate = predicate.And(interaction => (interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt) <= request.EndDate.Value);

        Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>> orderBy = (request.SortBy, request.SortDirection) switch
        {
            (InteractionSortField.InteractionDate, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.ScheduledAt, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.ScheduledAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.ScheduledAt, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.ScheduledAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.OccurredAt, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.OccurredAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.OccurredAt, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.OccurredAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.Status, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.Status).ThenBy(interaction => interaction.Id),
            (InteractionSortField.Status, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.Status).ThenBy(interaction => interaction.Id),
            (InteractionSortField.Channel, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.Channel).ThenBy(interaction => interaction.Id),
            (InteractionSortField.Channel, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.Channel).ThenBy(interaction => interaction.Id),
            (InteractionSortField.NextFollowUpAt, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.NextFollowUpAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.NextFollowUpAt, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.NextFollowUpAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.CreatedAt, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.CreatedAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.CreatedAt, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.CreatedAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.UpdatedAt, SortingDirection.Ascending) => query => query.OrderBy(interaction => interaction.UpdatedAt).ThenBy(interaction => interaction.Id),
            (InteractionSortField.UpdatedAt, SortingDirection.Descending) => query => query.OrderByDescending(interaction => interaction.UpdatedAt).ThenBy(interaction => interaction.Id),
            _ => query => query.OrderByDescending(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt).ThenBy(interaction => interaction.Id)
        };

        var totalCount = await interactionRepository.GetCount(predicate);
        var interactions = await interactionRepository.GetAllAsync(
            filter: predicate,
            orderBy: orderBy,
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
