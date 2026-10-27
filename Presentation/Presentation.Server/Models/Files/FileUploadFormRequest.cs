namespace Presentation.Server.Models.Files;

public sealed class FileUploadFormRequest
{
    public IFormFile File { get; init; } = default!;
}
