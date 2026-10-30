using Application.Features.CustomerFeature.DTOs;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Import;

public sealed record ImportCustomersCommand(IReadOnlyList<ImportCustomerRow> Rows) : IRequest<ImportCustomersResultDto>;

public sealed record ImportCustomerRow(
    int RowNumber,
    string? FullName,
    string? CompanyName,
    string? PhoneNumber,
    string? Email,
    string? CustomerType,
    string? Sector,
    string? Website,
    string? SourceUrl,
    string? Location,
    string? Note);
