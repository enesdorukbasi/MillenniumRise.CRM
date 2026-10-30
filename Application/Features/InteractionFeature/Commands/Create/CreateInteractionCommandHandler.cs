using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InteractionFeature.Commands.Create;

public sealed class CreateInteractionCommandHandler(IRepository<Interaction> interactionRepository, IRepository<CustomerProduct> customerProductRepository, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<CreateInteractionCommand, InteractionDto>
{
    public async Task<InteractionDto> Handle(CreateInteractionCommand request, CancellationToken cancellationToken)
    {
        var customerProduct = await customerProductRepository.SingleOrDefaultAsync(
            current => current.Id == request.CustomerProductId,
            query => query
                .Include(current => current.Customer)
                .Include(current => current.Product)
                .ThenInclude(product => product.ProductLogo))
            ?? throw new NotFoundException("Müşteri ürünü", request.CustomerProductId);

        var interaction = new Interaction
        {
            CustomerProductId = request.CustomerProductId,
            ScheduledAt = request.ScheduledAt,
            OccurredAt = request.OccurredAt,
            Channel = request.Channel,
            Status = request.Status,
            Outcome = request.Outcome,
            Notes = request.Notes,
            NextFollowUpAt = request.NextFollowUpAt
        };

        await interactionRepository.AddAsync(interaction);
        await interactionRepository.SaveChangesAsync();

        interaction.CustomerProduct = customerProduct;

        return interactionMapper.Map(interaction);
    }
}
