using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Extensions;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.CustomerFeature.Queries.GetList;

public sealed class GetCustomerListQueryHandler(IRepository<Customer> customerRepository, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<GetCustomerListQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Customer, bool>>? predicate = null;
        if (request.ProductId.HasValue && request.HasProduct.HasValue)
        {
            Expression<Func<Customer, bool>> productPredicate = customer => customer.CustomerProducts.Any(customerProduct => customerProduct.ProductId == request.ProductId.Value);
            predicate = request.HasProduct.Value
                ? predicate.And(productPredicate)
                : predicate.And(productPredicate.Not());
        }

        var customers = await customerRepository.GetAllAsync(filter: predicate, orderBy: query => query.OrderBy(customer => customer.FullName), asNoTracking: true, splitQuery: false);

        return customerMapper.Map(customers.ToList());
    }
}
