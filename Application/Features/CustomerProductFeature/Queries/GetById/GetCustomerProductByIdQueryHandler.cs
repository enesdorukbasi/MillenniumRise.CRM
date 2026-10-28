using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerProductFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerProductFeature.Queries.GetById;

public sealed class GetCustomerProductByIdQueryHandler(IRepository<CustomerProduct> customerProductRepository, IValidator<GetCustomerProductByIdQuery> validator, IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IRequestHandler<GetCustomerProductByIdQuery, CustomerProductDto>
{
    public async Task<CustomerProductDto> Handle(GetCustomerProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customerProduct = await customerProductRepository.SingleOrDefaultAsync(
            current => current.Id == request.Id,
            query => query
                .Include(current => current.Customer)
                .Include(current => current.Product)
                .ThenInclude(product => product.ProductLogo)
                .Include(current => current.Interactions.OrderByDescending(interaction => interaction.OccurredAt ?? interaction.ScheduledAt ?? interaction.CreatedAt).Take(1)))
            ?? throw new NotFoundException("Müşteri ürünü", request.Id);

        return customerProductMapper.Map(customerProduct);
    }
}
