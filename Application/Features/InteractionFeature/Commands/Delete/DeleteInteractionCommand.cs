using MediatR;

namespace Application.Features.InteractionFeature.Commands.Delete;

public sealed record DeleteInteractionCommand(Guid Id) : IRequest<Unit>;
