using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Features.CustomerFeature.DTOs;

public sealed record CustomerDto : BaseDto
{
    public required string FullName { get; init; }
    public string? CompanyName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public CustomerType CustomerType { get; init; }
    public string? Sector { get; init; }
    public string? Website { get; init; }
    public string? SourceUrl { get; init; }
    public string? Location { get; init; }
    public string? Note { get; init; }
}