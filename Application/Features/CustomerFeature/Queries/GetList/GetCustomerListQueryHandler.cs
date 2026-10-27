using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.CustomerFeature.Queries.GetList;

public sealed class GetCustomerListQueryHandler(IRepository<Customer> customerRepository, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<GetCustomerListQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync(orderBy: query => query.OrderBy(customer => customer.FullName), asNoTracking: true, splitQuery: false);
        return customerMapper.Map(customers.ToList());
    }
}