using Application.Features.ProductFeature.DTOs;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Update;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    Guid? ProductLogoId) : IRequest<ProductDto>;
