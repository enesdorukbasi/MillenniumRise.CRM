using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductFeature.Queries.GetList;

public sealed class GetProductListQueryHandler(IRepository<Product> productRepository, IMapper<Product, ProductDto> productMapper) : IRequestHandler<GetProductListQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(
            orderBy: query => query.OrderBy(product => product.Name),
            include: query => query.Include(product => product.ProductLogo),
            asNoTracking: true,
            splitQuery: false);

        return productMapper.Map(products.ToList());
    }
}
