using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InteractionFeature.Queries.GetById;

public sealed class GetInteractionByIdQueryHandler(IRepository<Interaction> interactionRepository, IValidator<GetInteractionByIdQuery> validator, IMapper<Interaction, InteractionDto> interactionMapper) : IRequestHandler<GetInteractionByIdQuery, InteractionDto>
{
    public async Task<InteractionDto> Handle(GetInteractionByIdQuery request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var interaction = await interactionRepository.SingleOrDefaultAsync(
            current => current.Id == request.Id,
            query => query
                .Include(current => current.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Customer)
                .Include(current => current.CustomerProduct)
                .ThenInclude(customerProduct => customerProduct.Product)
                .ThenInclude(product => product.ProductLogo))
            ?? throw new NotFoundException("Etkileşim", request.Id);

        return interactionMapper.Map(interaction);
    }
}
