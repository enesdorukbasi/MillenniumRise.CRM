using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Queries.GetById;

public sealed class GetCustomerByIdQueryHandler(IRepository<Customer> customerRepository, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri", request.Id);

        return customerMapper.Map(customer);
    }
}
