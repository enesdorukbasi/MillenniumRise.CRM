using Application.Abstractions.FileUploader;
using Application.Abstractions.Persistence.Repository;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.Delete;

public sealed class DeleteUploadedFileCommandHandler(IRepository<UploadedFile> uploadedFileRepository, IFileService fileService) : IRequestHandler<DeleteUploadedFileCommand, Guid>
{
    public async Task<Guid> Handle(DeleteUploadedFileCommand request, CancellationToken cancellationToken)
    {
        var uploadedFile = await uploadedFileRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Yüklenen dosya", request.Id);

        uploadedFileRepository.Remove(uploadedFile);
        await uploadedFileRepository.SaveChangesAsync();
        await fileService.DeleteAsync(uploadedFile.Url, cancellationToken);

        return uploadedFile.Id;
    }
}
