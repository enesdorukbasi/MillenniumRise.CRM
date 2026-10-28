using Application.Features.ProgramConfigurationFeature.DTOs;
using MediatR;

namespace Application.Features.ProgramConfigurationFeature.Commands.Update;

public sealed record UpdateProgramConfigurationCommand(string Name, Guid? LogoId, Guid? AuthPageImageId) : IRequest<ProgramConfigurationDto>;
