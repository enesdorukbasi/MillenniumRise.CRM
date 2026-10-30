using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Update;

public sealed class UpdateProductCommandHandler(IRepository<Product> productRepository, IRepository<UploadedFile> uploadedFileRepository, IMapper<Product, ProductDto> productMapper) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Ürün", request.Id);

        if (await productRepository.IsExistsAsync(current => current.Name == request.Name && current.Id != request.Id))
            throw new ConflictException("Aynı isimle kayıtlı başka bir ürün bulunuyor.");

        UploadedFile? productLogo = null;
        if (request.ProductLogoId.HasValue)
        {
            productLogo = await uploadedFileRepository.GetByIdAsync(request.ProductLogoId.Value)
                ?? throw new NotFoundException("Yüklenen dosya", request.ProductLogoId.Value);
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.IsActive = request.IsActive;
        product.ProductLogoId = request.ProductLogoId;
        product.ProductLogo = productLogo;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        productRepository.Update(product);
        await productRepository.SaveChangesAsync();

        return productMapper.Map(product);
    }
}
