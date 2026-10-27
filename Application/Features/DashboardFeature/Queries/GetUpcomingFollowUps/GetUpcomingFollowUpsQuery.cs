using Application.Common.Pagination;
using Application.Features.CustomerProductFeature.DTOs;
using MediatR;

namespace Application.Features.DashboardFeature.Queries.GetUpcomingFollowUps;

public sealed record GetUpcomingFollowUpsQuery(DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<CustomerProductDto>>;
