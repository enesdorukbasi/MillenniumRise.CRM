using Application.Abstractions.Mappers;
using Application.Features.CustomerFeature.DTOs;
using Application.Features.CustomerProductFeature.DTOs;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;

namespace Application.Features.CustomerProductFeature.Mappers;

public sealed class CustomerProductMapper(IMapper<Customer, CustomerDto> customerMapper, IMapper<Product, ProductDto> productMapper) : IMapper<CustomerProduct, CustomerProductDto>
{
    public CustomerProductDto Map(CustomerProduct entity)
    {
        return new CustomerProductDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CustomerId = entity.CustomerId,
            Customer = customerMapper.Map(entity.Customer),
            ProductId = entity.ProductId,
            Product = productMapper.Map(entity.Product),
            Status = entity.Status,
            NextFollowUpAt = entity.NextFollowUpAt,
            StatusNote = entity.StatusNote
        };
    }

    public List<CustomerProductDto> Map(List<CustomerProduct> entities)
    {
        return entities.Select(Map).ToList();
    }
}
