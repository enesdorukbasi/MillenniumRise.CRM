using Application.Abstractions.FileUploader.Models;

namespace Presentation.Server.Extensions;

public static class FileUploadRequestExtension
{
    public static FileUploadRequest ToFileUploadRequest(this IFormFile file)
    {
        return new FileUploadRequest
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length,
            OpenReadStream = file.OpenReadStream
        };
    }
}
