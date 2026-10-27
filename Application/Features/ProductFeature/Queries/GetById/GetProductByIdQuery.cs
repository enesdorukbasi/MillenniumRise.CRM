using Application.Features.ProductFeature.DTOs;
using MediatR;

namespace Application.Features.ProductFeature.Queries.GetById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
