using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.ProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ProductFeature.Queries.GetById;

public sealed class GetProductByIdQueryHandler(IRepository<Product> productRepository, IValidator<GetProductByIdQuery> validator, IMapper<Product, ProductDto> productMapper) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var product = await productRepository.GetByIdAsync(request.Id, product => product.ProductLogo!)
            ?? throw new NotFoundException("Ürün", request.Id);

        return productMapper.Map(product);
    }
}
