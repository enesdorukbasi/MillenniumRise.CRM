using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.UploadedFileFeature.Queries.GetById;

public sealed class GetUploadedFileByIdQueryHandler(IRepository<UploadedFile> uploadedFileRepository, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<GetUploadedFileByIdQuery, UploadedFileDto>
{
    public async Task<UploadedFileDto> Handle(GetUploadedFileByIdQuery request, CancellationToken cancellationToken)
    {
        var uploadedFile = await uploadedFileRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Yüklenen dosya", request.Id);

        return uploadedFileMapper.Map(uploadedFile);
    }
}
