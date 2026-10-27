using Application.Features.AppUserFeature.DTOs;
using MediatR;

namespace Application.Features.AppUserFeature.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;