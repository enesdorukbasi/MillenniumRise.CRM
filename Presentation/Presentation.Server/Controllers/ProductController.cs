using Application.Features.ProductFeature.Commands.Create;
using Application.Features.ProductFeature.Commands.Delete;
using Application.Features.ProductFeature.Commands.Update;
using Application.Features.ProductFeature.Queries.GetById;
using Application.Features.ProductFeature.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;
using System.Net;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class ProductController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetProductListQuery(), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return ApiResultFactory.Created(response).ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command with { Id = id }, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return ApiResultFactory.Success(HttpStatusCode.NoContent).ToActionResult();
    }
}
