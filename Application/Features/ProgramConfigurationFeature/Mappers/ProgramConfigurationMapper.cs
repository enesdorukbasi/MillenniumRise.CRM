using Application.Abstractions.Mappers;
using Application.Features.ProgramConfigurationFeature.DTOs;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;

namespace Application.Features.ProgramConfigurationFeature.Mappers;

public sealed class ProgramConfigurationMapper(IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IMapper<ProgramConfiguration, ProgramConfigurationDto>
{
    public ProgramConfigurationDto Map(ProgramConfiguration entity)
    {
        return new ProgramConfigurationDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Name = entity.Name,
            LogoId = entity.LogoId,
            Logo = entity.Logo is null ? null : uploadedFileMapper.Map(entity.Logo),
            AuthPageImageId = entity.AuthPageImageId,
            AuthPageImage = entity.AuthPageImage is null ? null : uploadedFileMapper.Map(entity.AuthPageImage)
        };
    }

    public List<ProgramConfigurationDto> Map(List<ProgramConfiguration> entities)
    {
        return entities.Select(Map).ToList();
    }
}
