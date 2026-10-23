using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Interaction : BaseEntity
{
    public Guid CustomerProductId { get; set; }
    public CustomerProduct CustomerProduct { get; set; } = null!;

    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? OccurredAt { get; set; }
    public InteractionChannel Channel { get; set; }
    public InteractionStatus Status { get; set; } = InteractionStatus.Planned;
    public InteractionOutcome? Outcome { get; set; }
    public required string Notes { get; set; }
    public DateTimeOffset? NextFollowUpAt { get; set; }
}
