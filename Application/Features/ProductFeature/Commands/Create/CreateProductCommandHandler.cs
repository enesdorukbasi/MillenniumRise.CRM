using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Create;

public sealed class CreateProductCommandHandler(IRepository<Product> productRepository, IRepository<UploadedFile> uploadedFileRepository, IMapper<Product, ProductDto> productMapper) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await productRepository.IsExistsAsync(product => product.Name == request.Name))
            throw new ConflictException("Aynı isimle kayıtlı başka bir ürün bulunuyor.");

        UploadedFile? productLogo = null;
        if (request.ProductLogoId.HasValue)
        {
            productLogo = await uploadedFileRepository.GetByIdAsync(request.ProductLogoId.Value)
                ?? throw new NotFoundException("Yüklenen dosya", request.ProductLogoId.Value);
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            ProductLogoId = request.ProductLogoId,
            ProductLogo = productLogo
        };

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        return productMapper.Map(product);
    }
}
