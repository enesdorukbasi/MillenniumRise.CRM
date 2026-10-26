using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Application.Features.AppUserFeature.DTOs;
using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.Models;

namespace Infrastructure.Authentication;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> options) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = options.Value;
    private readonly SymmetricSecurityKey _symmetricSecurityKey = CreateSecurityKey(options.Value.Key);

    internal static SymmetricSecurityKey CreateSecurityKey(string key)
    {
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(key);
        }
        catch
        {
            keyBytes = Encoding.UTF8.GetBytes(key);
        }

        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt anahtarı en az 32 bayt olmalıdır.");

        return new SymmetricSecurityKey(keyBytes);
    }

    public LoginResponseDto GenerateToken(AppUser user, IEnumerable<Claim>? extraClaims = null) => GenerateToken(user.Id, user.FullName, user.Email, user.PhoneNumber, extraClaims);

    private LoginResponseDto GenerateToken(Guid id, string? fullName, string? email, string? phoneNumber, IEnumerable<Claim>? extraClaims)
    {
        var signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, id.ToString()),
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, fullName ?? string.Empty),
            new(ClaimTypes.Email, email ?? string.Empty),
            new(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        if (extraClaims is not null)
            claims.AddRange(extraClaims);

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: signingCredentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expires
        };
    }
}
