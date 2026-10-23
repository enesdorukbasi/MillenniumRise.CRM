using Domain.Base;

namespace Domain.Entities;

public sealed class UploadedFile : BaseEntity
{
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required string Url { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? ImageVariantsJson { get; set; }
}