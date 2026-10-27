using Application.Common.Pagination;
using Application.Features.InteractionFeature.DTOs;
using MediatR;

namespace Application.Features.DashboardFeature.Queries.GetRecentInteractions;

public sealed record GetRecentInteractionsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<InteractionDto>>;
