namespace Application.Abstractions.FileUploader.Models;

public sealed record FileUploadRequest
{
    public required string FileName { get; init; }
    public string ContentType { get; init; } = "application/octet-stream";
    public long Length { get; init; }
    public required Func<Stream> OpenReadStream { get; init; }
}
