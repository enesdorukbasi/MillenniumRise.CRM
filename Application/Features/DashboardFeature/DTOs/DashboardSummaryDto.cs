namespace Application.Features.DashboardFeature.DTOs;

public sealed record DashboardSummaryDto
{
    public long TotalCustomerCount { get; init; }
    public long ActiveProductCount { get; init; }
    public long CustomerProductCount { get; init; }
    public long PendingFollowUpCount { get; init; }
}
