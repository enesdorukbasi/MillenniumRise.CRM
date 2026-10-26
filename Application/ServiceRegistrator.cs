using Application.Abstractions.Mappers;
using Application.Abstractions.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistrator
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatRServices();
        services.AddValidatorServices();
        services.AddMapperServices();
        return services;
    }

    private static void AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ServiceRegistrator).Assembly));
    }

    private static void AddValidatorServices(this IServiceCollection services)
    {
        services.AddGenericImplementations(typeof(IValidator<>));
    }

    private static void AddMapperServices(this IServiceCollection services)
    {
        services.AddGenericImplementations(typeof(IMapper<,>));
    }

    private static void AddGenericImplementations(this IServiceCollection services, Type openGenericServiceType)
    {
        var applicationAssembly = typeof(ServiceRegistrator).Assembly;

        foreach (var implementationType in applicationAssembly.DefinedTypes.Where(type => !type.IsAbstract && !type.IsInterface))
        {
            var serviceTypes = implementationType.ImplementedInterfaces.Where(serviceType =>
                serviceType.IsGenericType &&
                serviceType.GetGenericTypeDefinition() == openGenericServiceType);
            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }
    }
}
