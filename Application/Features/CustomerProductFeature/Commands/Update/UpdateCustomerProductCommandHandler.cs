using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerProductFeature.Commands.Update;

public sealed class UpdateCustomerProductCommandHandler(IRepository<CustomerProduct> customerProductRepository, IRepository<Customer> customerRepository, IRepository<Product> productRepository, IValidator<UpdateCustomerProductCommand> validator, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<UpdateCustomerProductCommand, CustomerProductDto>
{
    public async Task<CustomerProductDto> Handle(UpdateCustomerProductCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customerProduct = await customerProductRepository.SingleOrDefaultAsync(
            current => current.Id == request.Id,
            query => query
                .Include(current => current.Customer)
                .Include(current => current.Product)
                .ThenInclude(product => product.ProductLogo))
            ?? throw new NotFoundException("Müşteri ürünü", request.Id);

        var customer = await customerRepository.SingleOrDefaultAsync(current => current.Id == request.CustomerId)
            ?? throw new NotFoundException("Müşteri", request.CustomerId);

        var product = await productRepository.SingleOrDefaultAsync(
            current => current.Id == request.ProductId,
            query => query.Include(current => current.ProductLogo))
            ?? throw new NotFoundException("Ürün", request.ProductId);

        if (await customerProductRepository.IsExistsAsync(current =>
                current.Id != request.Id &&
                current.CustomerId == request.CustomerId &&
                current.ProductId == request.ProductId))
        {
            throw new ConflictException("Müşteri bu ürüne zaten sahip.");
        }

        customerProduct.CustomerId = request.CustomerId;
        customerProduct.Customer = customer;
        customerProduct.ProductId = request.ProductId;
        customerProduct.Product = product;
        customerProduct.Status = request.Status;
        customerProduct.NextFollowUpAt = request.NextFollowUpAt;
        customerProduct.StatusNote = request.StatusNote;
        customerProduct.UpdatedAt = DateTimeOffset.UtcNow;

        customerProductRepository.Update(customerProduct);
        await customerProductRepository.SaveChangesAsync();

        return customerProductMapper.Map(customerProduct);
    }
}
