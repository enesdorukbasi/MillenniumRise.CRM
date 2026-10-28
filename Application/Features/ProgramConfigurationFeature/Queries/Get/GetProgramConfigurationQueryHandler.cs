using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.ProgramConfigurationFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProgramConfigurationFeature.Queries.Get;

public sealed class GetProgramConfigurationQueryHandler(IRepository<ProgramConfiguration> programConfigurationRepository, IMapper<ProgramConfiguration, ProgramConfigurationDto> programConfigurationMapper) : IRequestHandler<GetProgramConfigurationQuery, ProgramConfigurationDto>
{
    public async Task<ProgramConfigurationDto> Handle(GetProgramConfigurationQuery request, CancellationToken cancellationToken)
    {
        var programConfiguration = await programConfigurationRepository.SingleOrDefaultAsync(
            configuration => true,
            query => query
                .Include(configuration => configuration.Logo)
                .Include(configuration => configuration.AuthPageImage))
            ?? throw new NotFoundException();

        return programConfigurationMapper.Map(programConfiguration);
    }
}
