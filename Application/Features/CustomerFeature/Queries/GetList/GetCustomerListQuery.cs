using Application.Features.CustomerFeature.DTOs;
using MediatR;

namespace Application.Features.CustomerFeature.Queries.GetList;

public sealed record GetCustomerListQuery : IRequest<List<CustomerDto>>;