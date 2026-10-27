using Application.Abstractions.FileUploader;
using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.UploadMany;

public sealed class UploadUploadedFilesCommandHandler(IRepository<UploadedFile> uploadedFileRepository, IFileService fileService, IValidator<UploadUploadedFilesCommand> validator, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<UploadUploadedFilesCommand, List<UploadedFileDto>>
{
    public async Task<List<UploadedFileDto>> Handle(UploadUploadedFilesCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

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
