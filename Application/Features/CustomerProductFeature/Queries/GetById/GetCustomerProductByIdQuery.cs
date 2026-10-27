using Application.Features.CustomerProductFeature.DTOs;
using MediatR;

namespace Application.Features.CustomerProductFeature.Queries.GetById;

public sealed record GetCustomerProductByIdQuery(Guid Id) : IRequest<CustomerProductDto>;
