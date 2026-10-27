using Application.Features.CustomerProductFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.CustomerProductFeature.Commands.Update;

public sealed record UpdateCustomerProductCommand(
    Guid Id,
    Guid CustomerId,
    Guid ProductId,
    CustomerProductStatus Status,
    DateTimeOffset? NextFollowUpAt,
    string? StatusNote) : IRequest<CustomerProductDto>;
