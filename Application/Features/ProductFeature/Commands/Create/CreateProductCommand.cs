using Application.Features.ProductFeature.DTOs;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Create;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    bool IsActive,
    Guid? ProductLogoId) : IRequest<ProductDto>;
