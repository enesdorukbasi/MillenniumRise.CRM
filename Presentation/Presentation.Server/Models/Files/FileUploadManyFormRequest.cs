namespace Presentation.Server.Models.Files;

public sealed class FileUploadManyFormRequest
{
    public List<IFormFile> Files { get; init; } = [];
}
