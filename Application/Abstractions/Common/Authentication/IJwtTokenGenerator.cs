using Application.Feature.AppUserFeature.DTOs;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Abstractions.Platform.Authentication;

public interface IJwtTokenGenerator
{
    LoginResponseDto GenerateToken(AppUser user, IEnumerable<Claim>? extraClaims = null);
}
