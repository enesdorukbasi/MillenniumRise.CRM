using Application.Features.CustomerFeature.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Create;

public sealed record CreateCustomerCommand(
    string FullName,
    string? CompanyName,
    string? PhoneNumber,
    string? Email,
    CustomerType CustomerType,
    string? Sector,
    string? Website,
    string? SourceUrl,
    string? Location,
    string? Note) : IRequest<CustomerDto>;