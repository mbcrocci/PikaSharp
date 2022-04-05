using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PikaSharp;

public static class PikaServiceCollectionExtensions
{
    public static IServiceCollection AddConnector<URL>(this IServiceCollection services) where URL : class, IUrlProvider
    {
        services.AddSingleton<IUrlProvider, URL>();
        services.AddSingleton<IConnector, Connector>();
        return services;
    }

    public static IServiceCollection AddPublisher<P>(this IServiceCollection services) where P : class
    {
        services.AddSingleton<P>();
        return services;
    }

    public static IServiceCollection AddConsumer<C>(this IServiceCollection services) where C : class, IHostedService
    {
        services.AddHostedService<C>();
        return services;
    }

    public static IServiceCollection AddNotifier<N>(this IServiceCollection services) where N : BackgroundService
    {
        services.AddHostedService<N>();
        return services;
    }
}
