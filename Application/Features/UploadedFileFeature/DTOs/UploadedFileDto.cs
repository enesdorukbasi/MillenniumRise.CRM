using Application.Common.DTOs;

namespace Application.Features.UploadedFileFeature.DTOs;

public sealed record UploadedFileDto : BaseDto
{
    public required string Name { get; init; }
    public required string OriginalFileName { get; init; }
    public required string Extension { get; init; }
    public required string Url { get; init; }
    public required string ContentType { get; init; }
    public long FileSize { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string? ImageVariantsJson { get; init; }
}
