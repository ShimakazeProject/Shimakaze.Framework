using Shimakaze.Framework;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, string applicationId)
        => services.AddApplication(ApplicationUtils.CreateApplication(applicationId));
}