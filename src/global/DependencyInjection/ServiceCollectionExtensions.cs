using Shimakaze.Framework;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
        => services.AddDispatcher(Platform.CreateDispatcher());

    public static IServiceCollection AddApplication(this IServiceCollection services, string applicationId)
        => services.AddApplication(provider => Platform.CreateApplication(provider.GetRequiredService<Dispatcher>(), applicationId));

    public static IServiceCollection AddShimakazeFramework(this IServiceCollection services, string applicationId)
        => services.AddDispatcher().AddApplication(applicationId);
}