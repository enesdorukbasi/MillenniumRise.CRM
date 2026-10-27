using Application.Features.InteractionFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.InteractionFeature.Queries.GetList;

public sealed record GetInteractionListQuery(
    Guid? CustomerProductId,
    InteractionStatus? Status,
    InteractionChannel? Channel,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate) : IRequest<List<InteractionDto>>;
