using Application.Abstractions.FileUploader.Models;
using Application.Features.UploadedFileFeature.DTOs;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.UploadMany;

public sealed record UploadUploadedFilesCommand : IRequest<List<UploadedFileDto>>
{
    public IReadOnlyCollection<FileUploadRequest> Files { get; init; } = [];
}
