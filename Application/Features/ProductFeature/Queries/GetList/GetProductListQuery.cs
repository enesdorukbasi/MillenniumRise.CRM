using Application.Features.ProductFeature.DTOs;
using MediatR;

namespace Application.Features.ProductFeature.Queries.GetList;

public sealed record GetProductListQuery : IRequest<List<ProductDto>>;
