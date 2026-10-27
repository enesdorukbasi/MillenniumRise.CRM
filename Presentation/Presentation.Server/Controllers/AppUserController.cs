using Application.Features.AppUserFeature.Commands;
using Application.Features.AppUserFeature.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;

namespace Presentation.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AppUserController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }
}
