using Application.Abstractions.FileUploader;
using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.Upload;

public sealed class UploadUploadedFileCommandHandler(IRepository<UploadedFile> uploadedFileRepository, IFileService fileService, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<UploadUploadedFileCommand, UploadedFileDto>
{
    public async Task<UploadedFileDto> Handle(UploadUploadedFileCommand request, CancellationToken cancellationToken)
    {
        var uploadedFile = await fileService.UploadAsync(request.File, cancellationToken);

        try
        {
            await uploadedFileRepository.AddAsync(uploadedFile);
            await uploadedFileRepository.SaveChangesAsync();
        }
        catch
        {
            await fileService.DeleteAsync(uploadedFile.Url, cancellationToken);
            throw;
        }

        return uploadedFileMapper.Map(uploadedFile);
    }
}
