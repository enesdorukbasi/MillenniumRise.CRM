using Application.Features.ProgramConfigurationFeature.DTOs;
using MediatR;

namespace Application.Features.ProgramConfigurationFeature.Queries.Get;

public sealed record GetProgramConfigurationQuery : IRequest<ProgramConfigurationDto>;
