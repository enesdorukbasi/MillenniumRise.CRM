using Application.Features.AppUserFeature.DTOs;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    LoginResponseDto GenerateToken(AppUser user, IEnumerable<Claim>? extraClaims = null);
}
