using MediatR;

namespace Application.Features.CustomerProductFeature.Commands.Delete;

public sealed record DeleteCustomerProductCommand(Guid Id) : IRequest<Unit>;
