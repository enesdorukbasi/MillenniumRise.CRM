using Application.Features.UploadedFileFeature.DTOs;
using MediatR;

namespace Application.Features.UploadedFileFeature.Queries.GetList;

public sealed record GetUploadedFileListQuery : IRequest<List<UploadedFileDto>>;
