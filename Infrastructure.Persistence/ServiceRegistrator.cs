using Application.Abstractions.Persistence.Repository;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Infrastructure.Persistence;

public static class ServiceRegistrator
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDependencyInjection();
        services.AddMySQL(configuration);
        return services;
    }
    private static void AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
    private static void AddMySQL(this IServiceCollection services, IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection ayarı bulunamadı.");
        var connectionStringBuilder = new MySqlConnectionStringBuilder(configuredConnectionString)
        {
            AllowUserVariables = true
        };

        services.AddDbContextFactory<CrmDbContext>(options => options.UseMySQL(connectionStringBuilder.ConnectionString));
    }
}
