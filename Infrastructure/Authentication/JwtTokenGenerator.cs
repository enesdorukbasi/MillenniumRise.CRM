using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.Models;
using Application.Features.AppUserFeature.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> options, SymmetricSecurityKey symmetricSecurityKey) : IJwtTokenGenerator
{
    public static SymmetricSecurityKey CreateSecurityKey(string key)
    {
        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(key);
        }
        catch (FormatException)
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
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
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

        var expires = DateTime.UtcNow.AddMinutes(options.Value.ExpiresInMinutes);
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
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
