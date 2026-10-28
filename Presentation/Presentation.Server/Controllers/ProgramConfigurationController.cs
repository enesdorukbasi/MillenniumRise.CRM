using Application.Features.ProgramConfigurationFeature.Commands.Update;
using Application.Features.ProgramConfigurationFeature.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class ProgramConfigurationController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetProgramConfigurationQuery(), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProgramConfigurationCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }
}
