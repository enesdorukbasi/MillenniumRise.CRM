using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerProductFeature.Commands.Create;

public sealed class CreateCustomerProductCommandHandler(IRepository<CustomerProduct> customerProductRepository, IRepository<Customer> customerRepository, IRepository<Product> productRepository, IValidator<CreateCustomerProductCommand> validator, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<CreateCustomerProductCommand, CustomerProductDto>
{
    public async Task<CustomerProductDto> Handle(CreateCustomerProductCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customer = await customerRepository.SingleOrDefaultAsync(current => current.Id == request.CustomerId)
            ?? throw new NotFoundException("Müşteri", request.CustomerId);

        var product = await productRepository.SingleOrDefaultAsync(
            current => current.Id == request.ProductId,
            query => query.Include(current => current.ProductLogo))
            ?? throw new NotFoundException("Ürün", request.ProductId);

        if (await customerProductRepository.IsExistsAsync(customerProduct =>
                customerProduct.CustomerId == request.CustomerId &&
                customerProduct.ProductId == request.ProductId))
        {
            throw new ConflictException("Müşteri bu ürüne zaten sahip.");
        }

        var customerProduct = new CustomerProduct
        {
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            Status = request.Status,
            NextFollowUpAt = request.NextFollowUpAt,
            StatusNote = request.StatusNote
        };

        await customerProductRepository.AddAsync(customerProduct);
        await customerProductRepository.SaveChangesAsync();

        customerProduct.Customer = customer;
        customerProduct.Product = product;

        return customerProductMapper.Map(customerProduct);
    }
}
