namespace Application.Abstractions.Persistence.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
