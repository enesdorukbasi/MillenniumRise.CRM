using Application.Features.InteractionFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.InteractionFeature.Commands.Create;

public sealed record CreateInteractionCommand(
    Guid CustomerProductId,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? OccurredAt,
    InteractionChannel Channel,
    InteractionStatus Status,
    InteractionOutcome? Outcome,
    string Notes,
    DateTimeOffset? NextFollowUpAt) : IRequest<InteractionDto>;
