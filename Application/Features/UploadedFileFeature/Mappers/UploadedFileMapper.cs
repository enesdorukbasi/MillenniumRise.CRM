using Application.Abstractions.Mappers;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;

namespace Application.Features.UploadedFileFeature.Mappers;

public sealed class UploadedFileMapper : IMapper<UploadedFile, UploadedFileDto>
{
    public UploadedFileDto Map(UploadedFile entity)
    {
        return new UploadedFileDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Name = entity.Name,
            OriginalFileName = entity.OriginalFileName,
            Extension = entity.Extension,
            Url = entity.Url,
            ContentType = entity.ContentType,
            FileSize = entity.FileSize,
            Width = entity.Width,
            Height = entity.Height,
            ImageVariantsJson = entity.ImageVariantsJson
        };
    }

    public List<UploadedFileDto> Map(List<UploadedFile> entities)
    {
        return entities.Select(Map).ToList();
    }
}
