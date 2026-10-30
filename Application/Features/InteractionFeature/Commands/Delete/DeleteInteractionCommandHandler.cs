using Application.Abstractions.Persistence.Repository;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.InteractionFeature.Commands.Delete;

public sealed class DeleteInteractionCommandHandler(IRepository<Interaction> interactionRepository) : IRequestHandler<DeleteInteractionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteInteractionCommand request, CancellationToken cancellationToken)
    {
        var interaction = await interactionRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Etkileşim", request.Id);

        interactionRepository.Remove(interaction);
        await interactionRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
