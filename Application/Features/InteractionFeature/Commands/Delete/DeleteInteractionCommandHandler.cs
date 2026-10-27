using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.InteractionFeature.Commands.Delete;

public sealed class DeleteInteractionCommandHandler(IRepository<Interaction> interactionRepository, IValidator<DeleteInteractionCommand> validator) : IRequestHandler<DeleteInteractionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteInteractionCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var interaction = await interactionRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Etkileşim", request.Id);

        interactionRepository.Remove(interaction);
        await interactionRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
