using Application.Common.DTOs;
using Application.Features.CustomerFeature.DTOs;
using Application.Features.ProductFeature.DTOs;
using Domain.Enums;

namespace Application.Features.CustomerProductFeature.DTOs;

public sealed record CustomerProductDto : BaseDto
{
    public Guid CustomerId { get; init; }
    public required CustomerDto Customer { get; init; }
    public Guid ProductId { get; init; }
    public required ProductDto Product { get; init; }
    public CustomerProductStatus Status { get; init; }
    public DateTimeOffset? NextFollowUpAt { get; init; }
    public string? StatusNote { get; init; }
}
