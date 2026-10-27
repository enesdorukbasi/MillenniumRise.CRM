using Application.Features.InteractionFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.InteractionFeature.Commands.Update;

public sealed record UpdateInteractionCommand(
    Guid Id,
    Guid CustomerProductId,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? OccurredAt,
    InteractionChannel Channel,
    InteractionStatus Status,
    InteractionOutcome? Outcome,
    string Notes,
    DateTimeOffset? NextFollowUpAt) : IRequest<InteractionDto>;
