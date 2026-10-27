namespace Application.Abstractions.FileUploader.Models;

public sealed record ConvertedFileResult
{
    public required string Name { get; init; }
    public required string OriginalFileName { get; init; }
    public required string Extension { get; init; }
    public required string ContentType { get; init; }
    public long FileSize { get; init; }
    public required string Url { get; init; }
    public required IReadOnlyList<string> GeneratedUrls { get; init; }
}
