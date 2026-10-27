using Application.Features.UploadedFileFeature.Commands.Delete;
using Application.Features.UploadedFileFeature.Commands.Upload;
using Application.Features.UploadedFileFeature.Commands.UploadMany;
using Application.Features.UploadedFileFeature.Queries.GetById;
using Application.Features.UploadedFileFeature.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;
using Presentation.Server.Extensions;
using Presentation.Server.Models.Files;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class UploadedFileController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetUploadedFileListQuery query, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetUploadedFileByIdQuery(id), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 104_857_600)]
    public async Task<IActionResult> Upload([FromForm] FileUploadFormRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UploadUploadedFileCommand
        {
            File = request.File.ToFileUploadRequest()
        }, cancellationToken);

        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpPost("batch")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 104_857_600)]
    public async Task<IActionResult> UploadMany([FromForm] FileUploadManyFormRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UploadUploadedFilesCommand
        {
            Files = request.Files.Select(file => file.ToFileUploadRequest()).ToList()
        }, cancellationToken);

        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new DeleteUploadedFileCommand(id), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }
}
