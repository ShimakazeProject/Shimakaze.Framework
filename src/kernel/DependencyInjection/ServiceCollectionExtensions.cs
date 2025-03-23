using Microsoft.Extensions.DependencyInjection.Extensions;

using Shimakaze.Framework;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatcher<TDispatcher>(this IServiceCollection services, TDispatcher dispatcher)
        where TDispatcher : Dispatcher
    {
        services.TryAddSingleton<Dispatcher>(dispatcher);
        return services;
    }

    public static IServiceCollection AddApplication<TApplication>(this IServiceCollection services, TApplication application)
        where TApplication : Application
    {
        return services.AddApplication(_ => application);
    }

    public static IServiceCollection AddApplication<TApplication>(this IServiceCollection services, Func<IServiceProvider, TApplication> application)
        where TApplication : Application
    {
        services.TryAddSingleton<Application>(application);
        return services;
    }
}
