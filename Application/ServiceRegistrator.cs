using Application.Abstractions.Mappers;
using Application.Abstractions.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistrator
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediatRServices()
            .AddValidatorServices()
            .AddMapperServices();
    }

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        return services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ServiceRegistrator).Assembly));
    }

    private static IServiceCollection AddValidatorServices(this IServiceCollection services)
    {
        return services.AddGenericImplementations(typeof(IValidator<>));
    }

    private static IServiceCollection AddMapperServices(this IServiceCollection services)
    {
        return services.AddGenericImplementations(typeof(IMapper<,>));
    }

    private static IServiceCollection AddGenericImplementations(this IServiceCollection services, Type openGenericServiceType)
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
        return services;
    }
}
