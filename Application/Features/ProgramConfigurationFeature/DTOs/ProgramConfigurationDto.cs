using Application.Common.DTOs;
using Application.Features.UploadedFileFeature.DTOs;

namespace Application.Features.ProgramConfigurationFeature.DTOs;

public sealed record ProgramConfigurationDto : BaseDto
{
    public required string Name { get; init; }
    public Guid? LogoId { get; init; }
    public UploadedFileDto? Logo { get; init; }
    public Guid? AuthPageImageId { get; init; }
    public UploadedFileDto? AuthPageImage { get; init; }
}
