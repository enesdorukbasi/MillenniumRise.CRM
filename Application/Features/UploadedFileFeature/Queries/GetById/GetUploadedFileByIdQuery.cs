using Application.Features.UploadedFileFeature.DTOs;
using MediatR;

namespace Application.Features.UploadedFileFeature.Queries.GetById;

public sealed record GetUploadedFileByIdQuery(Guid Id) : IRequest<UploadedFileDto>;
