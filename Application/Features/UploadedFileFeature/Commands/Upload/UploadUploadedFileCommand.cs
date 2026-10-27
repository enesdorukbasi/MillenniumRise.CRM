using Application.Abstractions.FileUploader.Models;
using Application.Features.UploadedFileFeature.DTOs;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.Upload;

public sealed record UploadUploadedFileCommand : IRequest<UploadedFileDto>
{
    public required FileUploadRequest File { get; init; }
}
