using Application.Features.CustomerFeature.Commands.Create;
using Application.Features.CustomerFeature.Commands.Delete;
using Application.Features.CustomerFeature.Commands.Import;
using Application.Features.CustomerFeature.Commands.Update;
using Application.Features.CustomerFeature.Queries.GetById;
using Application.Features.CustomerFeature.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;
using Presentation.Server.Models.Customers;
using Presentation.Server.Services;
using System.Net;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class CustomerController(ISender sender, CustomerExcelService customerExcelService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetCustomerListQuery query, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpGet("import-template")]
    public IActionResult DownloadImportTemplate()
    {
        var content = customerExcelService.CreateTemplate();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "musteri-ice-aktarma-sablonu.xlsx");
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 5_242_880)]
    public async Task<IActionResult> Import([FromForm] CustomerImportFormRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            throw new Domain.Exceptions.AppValidationException("İçe aktarılacak Excel dosyasını seçin.");
        if (request.File.Length > 5_242_880)
            throw new Domain.Exceptions.AppValidationException("Excel dosyası en fazla 5 MB olabilir.");
        if (!string.Equals(Path.GetExtension(request.File.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new Domain.Exceptions.AppValidationException("Yalnızca .xlsx uzantılı Excel dosyaları yüklenebilir.");

        await using var stream = request.File.OpenReadStream();
        var rows = customerExcelService.ReadRows(stream);
        var response = await sender.Send(new ImportCustomersCommand(rows), cancellationToken);
        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command with { Id = id }, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCustomerCommand(id), cancellationToken);
        return ApiResultFactory.Success(HttpStatusCode.NoContent).ToActionResult();
    }
}
