using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.Models;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ServiceRegistrator
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDependencyInjections()
            .AddJwtServices(configuration);
    }
    private static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }
    private static IServiceCollection AddJwtServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetRequiredSection(JwtSettings.SectionName);
        JwtSettings jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt ayarları okunamadı.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
            string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
            string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
            jwtSettings.ExpiresInMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt ayarları eksik veya geçersiz.");
        }

        var signingKey = JwtTokenGenerator.CreateSecurityKey(jwtSettings.Key);

        services.Configure<JwtSettings>(jwtSection);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
        services.AddAuthorization();
        return services;
    }
}
