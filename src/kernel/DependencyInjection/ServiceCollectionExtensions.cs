using Microsoft.Extensions.DependencyInjection.Extensions;

using Shimakaze.Framework;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication<TApplication>(this IServiceCollection services, TApplication application)
        where TApplication : Application
    {
        services.TryAddSingleton<Application>(application);
        services.TryAddSingleton(provider => provider.GetRequiredService<Application>().Dispatcher);
        return services;
    }
}
