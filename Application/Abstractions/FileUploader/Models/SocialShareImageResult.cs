namespace Application.Abstractions.FileUploader.Models;

public sealed record SocialShareImageResult
{
    public required string Url { get; init; }
    public required string ContentType { get; init; }
    public required string Extension { get; init; }
    public long FileSize { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
}
