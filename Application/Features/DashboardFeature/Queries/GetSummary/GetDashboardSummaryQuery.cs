using Application.Features.DashboardFeature.DTOs;
using MediatR;

namespace Application.Features.DashboardFeature.Queries.GetSummary;

public sealed record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;
