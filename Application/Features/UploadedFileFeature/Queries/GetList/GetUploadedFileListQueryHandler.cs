using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.UploadedFileFeature.Queries.GetList;

public sealed class GetUploadedFileListQueryHandler(IRepository<UploadedFile> uploadedFileRepository, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<GetUploadedFileListQuery, List<UploadedFileDto>>
{
    public async Task<List<UploadedFileDto>> Handle(GetUploadedFileListQuery request, CancellationToken cancellationToken)
    {
        var uploadedFiles = await uploadedFileRepository.GetAllAsync(
            orderBy: query => query.OrderByDescending(file => file.CreatedAt),
            asNoTracking: true,
            splitQuery: false);

        return uploadedFileMapper.Map(uploadedFiles.ToList());
    }
}
