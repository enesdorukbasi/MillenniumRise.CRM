using Application.Features.InteractionFeature.DTOs;
using MediatR;

namespace Application.Features.InteractionFeature.Queries.GetById;

public sealed record GetInteractionByIdQuery(Guid Id) : IRequest<InteractionDto>;
