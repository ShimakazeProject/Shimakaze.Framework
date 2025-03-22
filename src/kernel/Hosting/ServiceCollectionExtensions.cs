using Microsoft.Extensions.DependencyInjection;

namespace Shimakaze.Framework.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseShimakazeFramework(this IServiceCollection services, EventHandler initialize)
    {
        services
            .AddSingleton(new ShimakazeFrameworkServiceOptions()
            {
                OnInitialize = initialize
            })
            .AddHostedService<ShimakazeFrameworkService>();

        return services;
    }
}