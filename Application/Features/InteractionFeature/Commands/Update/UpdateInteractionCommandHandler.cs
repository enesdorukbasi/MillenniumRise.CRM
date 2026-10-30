using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InteractionFeature.Commands.Update;

public sealed class UpdateInteractionCommandHandler(IRepository<Interaction> interactionRepository, IRepository<CustomerProduct> customerProductRepository, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<UpdateInteractionCommand, InteractionDto>
{
    public async Task<InteractionDto> Handle(UpdateInteractionCommand request, CancellationToken cancellationToken)
    {
        var interaction = await interactionRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Etkileşim", request.Id);

        var customerProduct = await customerProductRepository.SingleOrDefaultAsync(
            current => current.Id == request.CustomerProductId,
            query => query
                .Include(current => current.Customer)
                .Include(current => current.Product)
                .ThenInclude(product => product.ProductLogo))
            ?? throw new NotFoundException("Müşteri ürünü", request.CustomerProductId);

        interaction.CustomerProductId = request.CustomerProductId;
        interaction.CustomerProduct = customerProduct;
        interaction.ScheduledAt = request.ScheduledAt;
        interaction.OccurredAt = request.OccurredAt;
        interaction.Channel = request.Channel;
        interaction.Status = request.Status;
        interaction.Outcome = request.Outcome;
        interaction.Notes = request.Notes;
        interaction.NextFollowUpAt = request.NextFollowUpAt;
        interaction.UpdatedAt = DateTimeOffset.UtcNow;

        interactionRepository.Update(interaction);
        await interactionRepository.SaveChangesAsync();

        return interactionMapper.Map(interaction);
    }
}
