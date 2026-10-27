using Application.Common.DTOs;
using Application.Features.UploadedFileFeature.DTOs;

namespace Application.Features.ProductFeature.DTOs;

public sealed record ProductDto : BaseDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public Guid? ProductLogoId { get; init; }
    public UploadedFileDto? ProductLogo { get; init; }
}
