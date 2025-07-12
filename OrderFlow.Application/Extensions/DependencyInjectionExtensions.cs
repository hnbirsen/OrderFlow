using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Domain.Dependencies;
using System.Reflection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        RegisterByLifetime<IScoped>(services, assemblies, ServiceLifetime.Scoped);
        RegisterByLifetime<ITransient>(services, assemblies, ServiceLifetime.Transient);
        RegisterByLifetime<ISingleton>(services, assemblies, ServiceLifetime.Singleton);

        return services;
    }

    private static void RegisterByLifetime<TLifetime>(
        IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime)
    {
        var interfaceType = typeof(TLifetime);
        var types = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsInterface == false &&
                x.IsAbstract == false &&
                interfaceType.IsAssignableFrom(x));

        foreach (var implementationType in types)
        {
            var interfaceToBind = implementationType
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i != interfaceType &&
                    interfaceType.IsAssignableFrom(i));

            if (interfaceToBind != null)
            {
                services.Add(new ServiceDescriptor(interfaceToBind, implementationType, lifetime));
            }
        }
    }
}
