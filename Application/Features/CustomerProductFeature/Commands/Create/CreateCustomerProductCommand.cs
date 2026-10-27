using Application.Features.CustomerProductFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.CustomerProductFeature.Commands.Create;

public sealed record CreateCustomerProductCommand(
    Guid CustomerId,
    Guid ProductId,
    CustomerProductStatus Status,
    DateTimeOffset? NextFollowUpAt,
    string? StatusNote) : IRequest<CustomerProductDto>;
