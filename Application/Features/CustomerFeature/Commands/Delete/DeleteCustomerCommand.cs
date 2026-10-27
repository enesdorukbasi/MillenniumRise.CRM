using MediatR;

namespace Application.Features.CustomerFeature.Commands.Delete;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Unit>;
