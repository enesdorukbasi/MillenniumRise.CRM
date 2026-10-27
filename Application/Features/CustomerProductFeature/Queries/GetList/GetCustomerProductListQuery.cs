using Application.Features.CustomerProductFeature.DTOs;
using MediatR;

namespace Application.Features.CustomerProductFeature.Queries.GetList;

public sealed record GetCustomerProductListQuery : IRequest<List<CustomerProductDto>>;
