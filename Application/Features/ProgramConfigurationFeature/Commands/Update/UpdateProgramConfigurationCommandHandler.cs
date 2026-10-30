using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.ProgramConfigurationFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProgramConfigurationFeature.Commands.Update;

public sealed class UpdateProgramConfigurationCommandHandler(IRepository<ProgramConfiguration> programConfigurationRepository, IRepository<UploadedFile> uploadedFileRepository, IMapper<ProgramConfiguration, ProgramConfigurationDto> programConfigurationMapper) : IRequestHandler<UpdateProgramConfigurationCommand, ProgramConfigurationDto>
{
    public async Task<ProgramConfigurationDto> Handle(UpdateProgramConfigurationCommand request, CancellationToken cancellationToken)
    {
        var programConfiguration = await programConfigurationRepository.SingleOrDefaultAsync(
            configuration => true,
            query => query
                .Include(configuration => configuration.Logo)
                .Include(configuration => configuration.AuthPageImage))
            ?? throw new NotFoundException();

        UploadedFile? logo = null;
        if (request.LogoId.HasValue)
        {
            logo = await uploadedFileRepository.GetByIdAsync(request.LogoId.Value)
                ?? throw new NotFoundException("Logo dosyası", request.LogoId.Value);
        }

        UploadedFile? authPageImage = null;
        if (request.AuthPageImageId.HasValue)
        {
            authPageImage = await uploadedFileRepository.GetByIdAsync(request.AuthPageImageId.Value)
                ?? throw new NotFoundException("Giriş sayfası görseli", request.AuthPageImageId.Value);
        }

        programConfiguration.Name = request.Name;
        programConfiguration.LogoId = request.LogoId;
        programConfiguration.Logo = logo;
        programConfiguration.AuthPageImageId = request.AuthPageImageId;
        programConfiguration.AuthPageImage = authPageImage;
        programConfiguration.UpdatedAt = DateTimeOffset.UtcNow;

        programConfigurationRepository.Update(programConfiguration);
        await programConfigurationRepository.SaveChangesAsync();

        return programConfigurationMapper.Map(programConfiguration);
    }
}
