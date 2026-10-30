using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Common.Pagination;
using Application.Extensions;
using Application.Features.UploadedFileFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.UploadedFileFeature.Queries.GetList;

public sealed class GetUploadedFileListQueryHandler(IRepository<UploadedFile> uploadedFileRepository, IMapper<UploadedFile, UploadedFileDto> uploadedFileMapper) : IRequestHandler<GetUploadedFileListQuery, PagedResult<UploadedFileDto>>
{
    public async Task<PagedResult<UploadedFileDto>> Handle(GetUploadedFileListQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<UploadedFile, bool>>? predicate = null;

        if (request.Search.IsNotNullOrEmpty())
        {
            predicate = predicate.And(file =>
                file.Name.Contains(request.Search!) ||
                file.OriginalFileName.Contains(request.Search!));
        }

        if (request.Extension.IsNotNullOrEmpty())
            predicate = predicate.And(file => file.Extension == request.Extension);

        if (request.ContentType.IsNotNullOrEmpty())
            predicate = predicate.And(file => file.ContentType == request.ContentType);

        if (request.MinimumFileSize.HasValue)
            predicate = predicate.And(file => file.FileSize >= request.MinimumFileSize.Value);

        if (request.MaximumFileSize.HasValue)
            predicate = predicate.And(file => file.FileSize <= request.MaximumFileSize.Value);

        if (request.CreatedFrom.HasValue)
            predicate = predicate.And(file => file.CreatedAt >= request.CreatedFrom.Value);

        if (request.CreatedTo.HasValue)
            predicate = predicate.And(file => file.CreatedAt <= request.CreatedTo.Value);

        Func<IQueryable<UploadedFile>, IOrderedQueryable<UploadedFile>> orderBy = (request.SortBy, request.SortDirection) switch
        {
            (UploadedFileSortField.Name, SortingDirection.Ascending) => query => query.OrderBy(file => file.Name).ThenBy(file => file.Id),
            (UploadedFileSortField.Name, SortingDirection.Descending) => query => query.OrderByDescending(file => file.Name).ThenBy(file => file.Id),
            (UploadedFileSortField.OriginalFileName, SortingDirection.Ascending) => query => query.OrderBy(file => file.OriginalFileName).ThenBy(file => file.Id),
            (UploadedFileSortField.OriginalFileName, SortingDirection.Descending) => query => query.OrderByDescending(file => file.OriginalFileName).ThenBy(file => file.Id),
            (UploadedFileSortField.Extension, SortingDirection.Ascending) => query => query.OrderBy(file => file.Extension).ThenBy(file => file.Id),
            (UploadedFileSortField.Extension, SortingDirection.Descending) => query => query.OrderByDescending(file => file.Extension).ThenBy(file => file.Id),
            (UploadedFileSortField.ContentType, SortingDirection.Ascending) => query => query.OrderBy(file => file.ContentType).ThenBy(file => file.Id),
            (UploadedFileSortField.ContentType, SortingDirection.Descending) => query => query.OrderByDescending(file => file.ContentType).ThenBy(file => file.Id),
            (UploadedFileSortField.FileSize, SortingDirection.Ascending) => query => query.OrderBy(file => file.FileSize).ThenBy(file => file.Id),
            (UploadedFileSortField.FileSize, SortingDirection.Descending) => query => query.OrderByDescending(file => file.FileSize).ThenBy(file => file.Id),
            (UploadedFileSortField.CreatedAt, SortingDirection.Ascending) => query => query.OrderBy(file => file.CreatedAt).ThenBy(file => file.Id),
            (UploadedFileSortField.UpdatedAt, SortingDirection.Ascending) => query => query.OrderBy(file => file.UpdatedAt).ThenBy(file => file.Id),
            (UploadedFileSortField.UpdatedAt, SortingDirection.Descending) => query => query.OrderByDescending(file => file.UpdatedAt).ThenBy(file => file.Id),
            _ => query => query.OrderByDescending(file => file.CreatedAt).ThenBy(file => file.Id)
        };

        var totalCount = await uploadedFileRepository.GetCount(predicate);
        var uploadedFiles = await uploadedFileRepository.GetAllAsync(
            filter: predicate,
            orderBy: orderBy,
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            asNoTracking: true,
            splitQuery: false);

        return new PagedResult<UploadedFileDto>(
            uploadedFileMapper.Map(uploadedFiles.ToList()),
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
