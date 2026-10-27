using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerProductFeature.Queries.GetList;

public sealed class GetCustomerProductListQueryHandler(IRepository<CustomerProduct> customerProductRepository, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<GetCustomerProductListQuery, List<CustomerProductDto>>
{
    public async Task<List<CustomerProductDto>> Handle(GetCustomerProductListQuery request, CancellationToken cancellationToken)
    {
        var customerProducts = await customerProductRepository.GetAllAsync(
            orderBy: query => query
                .OrderBy(customerProduct => customerProduct.Customer.FullName)
                .ThenBy(customerProduct => customerProduct.Product.Name),
            include: query => query
                .Include(customerProduct => customerProduct.Customer)
                .Include(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo),
            asNoTracking: true,
            splitQuery: false);

        return customerProductMapper.Map(customerProducts.ToList());
    }
}
