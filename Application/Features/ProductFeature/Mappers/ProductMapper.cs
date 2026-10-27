using Application.Abstractions.Mappers;
using Application.Features.ProductFeature.DTOs;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;

namespace Application.Features.ProductFeature.Mappers;

public sealed class ProductMapper(IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IMapper<Product, ProductDto>
{
    public ProductDto Map(Product entity)
    {
        return new ProductDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            ProductLogoId = entity.ProductLogoId,
            ProductLogo = entity.ProductLogo is null ? null : uploadedFileMapper.Map(entity.ProductLogo)
        };
    }

    public List<ProductDto> Map(List<Product> entities)
    {
        return entities.Select(Map).ToList();
    }
}
