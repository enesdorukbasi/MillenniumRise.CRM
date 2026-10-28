using Application.Abstractions.Persistence.Seeders;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
