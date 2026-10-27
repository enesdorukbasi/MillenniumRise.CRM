using Application.Abstractions.Mappers;
using Application.Features.CustomerProductFeature.DTOs;
using Application.Features.InteractionFeature.DTOs;
using Domain.Entities;

namespace Application.Features.InteractionFeature.Mappers;

public sealed class InteractionMapper(IMapper<CustomerProduct, CustomerProductDto> customerProductMapper) : IMapper<Interaction, InteractionDto>
{
    public InteractionDto Map(Interaction entity)
    {
        return new InteractionDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CustomerProductId = entity.CustomerProductId,
            CustomerProduct = customerProductMapper.Map(entity.CustomerProduct),
            ScheduledAt = entity.ScheduledAt,
            OccurredAt = entity.OccurredAt,
            Channel = entity.Channel,
            Status = entity.Status,
            Outcome = entity.Outcome,
            Notes = entity.Notes,
            NextFollowUpAt = entity.NextFollowUpAt
        };
    }

    public List<InteractionDto> Map(List<Interaction> entities)
    {
        return entities.Select(Map).ToList();
    }
}
