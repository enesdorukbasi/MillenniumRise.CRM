using Application.Abstractions.FileUploader;
using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.UploadMany;

public sealed class UploadUploadedFilesCommandHandler(IRepository<UploadedFile> uploadedFileRepository, IFileService fileService, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<UploadUploadedFilesCommand, List<UploadedFileDto>>
{
    public async Task<List<UploadedFileDto>> Handle(UploadUploadedFilesCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyList<UploadedFile> uploadedFiles = [];

        try
        {
            uploadedFiles = await fileService.UploadAsync(request.Files, cancellationToken);

            foreach (var uploadedFile in uploadedFiles)
                await uploadedFileRepository.AddAsync(uploadedFile);

            await uploadedFileRepository.SaveChangesAsync();
        }
        catch
        {
            foreach (var uploadedFile in uploadedFiles)
            {
                try
                {
                    await fileService.DeleteAsync(uploadedFile.Url, CancellationToken.None);
                }
                catch
                {
                }
            }

            throw;
        }

        return uploadedFileMapper.Map(uploadedFiles.ToList());
    }
}
