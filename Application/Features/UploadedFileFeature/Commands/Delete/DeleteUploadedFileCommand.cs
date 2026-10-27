using MediatR;

namespace Application.Features.UploadedFileFeature.Commands.Delete;

public sealed record DeleteUploadedFileCommand(Guid Id) : IRequest<Guid>;
