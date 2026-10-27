using MediatR;

namespace Application.Features.ProductFeature.Commands.Delete;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>;
