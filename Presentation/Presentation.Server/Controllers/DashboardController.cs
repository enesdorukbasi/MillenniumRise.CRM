using Application.Features.DashboardFeature.Queries.GetRecentInteractions;
using Application.Features.DashboardFeature.Queries.GetSummary;
using Application.Features.DashboardFeature.Queries.GetUpcomingFollowUps;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Server.Common.ApiResult;

namespace Presentation.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class DashboardController(ISender sender) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetDashboardSummaryQuery(), cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("upcoming-follow-ups")]
    public async Task<IActionResult> GetUpcomingFollowUps([FromQuery] GetUpcomingFollowUpsQuery query, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

    [HttpGet("recent-interactions")]
    public async Task<IActionResult> GetRecentInteractions([FromQuery] GetRecentInteractionsQuery query, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return ApiResultFactory.Success(response).ToActionResult();
    }

}
