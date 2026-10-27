using Application.Common.Pagination;
using Application.Features.InteractionFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.InteractionFeature.Queries.GetList;

public sealed record GetInteractionListQuery(
    string? Search = null,
    Guid? CustomerProductId = null,
    InteractionStatus? Status = null,
    InteractionChannel? Channel = null,
    InteractionOutcome? Outcome = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    InteractionSortField SortBy = InteractionSortField.InteractionDate,
    SortingDirection SortDirection = SortingDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<InteractionDto>>;

public enum InteractionSortField
{
    InteractionDate,
    ScheduledAt,
    OccurredAt,
    Status,
    Channel,
    NextFollowUpAt,
    CreatedAt,
    UpdatedAt
}
