using Application.Abstractions.FileUploader.Models;
using Domain.Entities;

namespace Application.Abstractions.FileUploader;

public interface IFileService
{
    Task<UploadedFile> UploadAsync(FileUploadRequest file, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UploadedFile>> UploadAsync(IEnumerable<FileUploadRequest> files, CancellationToken cancellationToken = default);
    Task<ConvertedFileResult> CreateWebPAsync(UploadedFile sourceFile, int quality = 82, CancellationToken cancellationToken = default);
    Task<SocialShareImageResult> CreateSocialShareImageAsync(UploadedFile sourceFile, int width = 1200, int height = 630, int quality = 88, CancellationToken cancellationToken = default);
    string GetSocialShareImageUrl(UploadedFile sourceFile, int width = 1200, int height = 630);
    Task<bool> DeleteAsync(string fileNameOrUrl, CancellationToken cancellationToken = default);
    bool Exists(string fileNameOrUrl);
    string GetUrl(string fileNameOrUrl);
}
