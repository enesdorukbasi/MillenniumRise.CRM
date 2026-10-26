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
        services.AddDependencyServices();
        services.AddMySqlServices(configuration);
        return services;
    }

    private static void AddDependencyServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    private static void AddMySqlServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<CrmDbContext>(options => options.UseMySQL(configuration.GetConnectionString("DefaultConnection") ?? ""));
    }
}
