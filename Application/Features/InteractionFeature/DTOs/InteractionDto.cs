using Application.Common.DTOs;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Enums;

namespace Application.Features.InteractionFeature.DTOs;

public sealed record InteractionDto : BaseDto
{
    public Guid CustomerProductId { get; init; }
    public required CustomerProductDto CustomerProduct { get; init; }
    public DateTimeOffset? ScheduledAt { get; init; }
    public DateTimeOffset? OccurredAt { get; init; }
    public InteractionChannel Channel { get; init; }
    public InteractionStatus Status { get; init; }
    public InteractionOutcome? Outcome { get; init; }
    public required string Notes { get; init; }
    public DateTimeOffset? NextFollowUpAt { get; init; }
}
