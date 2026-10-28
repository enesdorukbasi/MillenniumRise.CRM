using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence.Seeders;
using Application.Abstractions.Persistence.Seeders.Models;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Seeders;

public sealed class DatabaseSeeder(CrmDbContext context, IPasswordHasher passwordHasher, IOptions<AdminUserSettings> options, ILogger<DatabaseSeeder> logger) : IDatabaseSeeder
{
    public Guid AdminIdValue { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await AdminUserSeedAsync(cancellationToken);
        await ProgramConfigurationSeedAsync(cancellationToken);
    }

    private async Task ProgramConfigurationSeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.ProgramConfigurations.AnyAsync(cancellationToken))
            return;

        var programConfiguration = new ProgramConfiguration
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "MillenniumRise | CRM",
            LogoId = null,
            AuthPageImageId = null
        };

        await context.ProgramConfigurations.AddAsync(programConfiguration, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Başlangıç program ayarları oluşturuldu. ProgramConfigurationId: {ProgramConfigurationId}", programConfiguration.Id);
    }

    private async Task AdminUserSeedAsync(CancellationToken cancellationToken = default)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.FullName))
            throw new InvalidOperationException("AdminUser:FullName ayarı zorunludur.");

        if (string.IsNullOrWhiteSpace(settings.Email))
            throw new InvalidOperationException("AdminUser:Email ayarı zorunludur.");

        if (string.IsNullOrWhiteSpace(settings.Password))
            throw new InvalidOperationException("AdminUser:Password ayarı zorunludur.");

        var appUser = await context.AppUsers
            .SingleOrDefaultAsync(user => user.Id == AdminIdValue, cancellationToken);

        if (appUser is null)
        {
            var emailExists = await context.AppUsers
                .AnyAsync(user => user.Email == settings.Email, cancellationToken);

            if (emailExists)
                throw new InvalidOperationException($"'{settings.Email}' e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

            var (hash, saltBase64) = passwordHasher.Hash(settings.Password);

            appUser = new AppUser
            {
                Id = AdminIdValue,
                FullName = settings.FullName,
                Email = settings.Email,
                HashedPassword = hash,
                PasswordSalt = saltBase64
            };

            await context.AppUsers.AddAsync(appUser, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Başlangıç admin kullanıcısı oluşturuldu. UserId: {UserId}, Email: {Email}", appUser.Id, appUser.Email);
            return;
        }

        var hasChanges = false;

        if (!string.Equals(appUser.FullName, settings.FullName, StringComparison.Ordinal))
        {
            appUser.FullName = settings.FullName;
            hasChanges = true;
        }

        if (!string.Equals(appUser.Email, settings.Email, StringComparison.Ordinal))
        {
            appUser.Email = settings.Email;
            hasChanges = true;
        }

        if (!passwordHasher.Verify(settings.Password, appUser.HashedPassword, appUser.PasswordSalt))
        {
            var (hash, saltBase64) = passwordHasher.Hash(settings.Password);
            appUser.HashedPassword = hash;
            appUser.PasswordSalt = saltBase64;
            hasChanges = true;
        }

        if (!hasChanges)
            return;

        appUser.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Başlangıç admin kullanıcısı güncellendi. UserId: {UserId}, Email: {Email}", appUser.Id, appUser.Email);
    }
}
