using Application.Features.CustomerFeature.DTOs;
using MediatR;

namespace Application.Features.CustomerFeature.Queries.GetById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;
