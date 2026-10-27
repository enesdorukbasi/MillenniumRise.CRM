using Application.Features.CustomerProductFeature.Commands.Create;
using Application.Features.CustomerProductFeature.Commands.Delete;
using Application.Features.CustomerProductFeature.Commands.Update;
using Application.Features.CustomerProductFeature.Queries.GetById;
using Application.Features.CustomerProductFeature.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;
using System.Net;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class CustomerProductController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetCustomerProductListQuery query, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetCustomerProductByIdQuery(id), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerProductCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerProductCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command with { Id = id }, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCustomerProductCommand(id), cancellationToken);
        return ApiResultFactory.Success(HttpStatusCode.NoContent).ToActionResult();
    }
}
