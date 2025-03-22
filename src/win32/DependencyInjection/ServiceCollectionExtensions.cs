using System.Runtime.Versioning;

using Shimakaze.Framework.Win32;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    [SupportedOSPlatform("windows5.0")]
    public static IServiceCollection AddWin32Application(this IServiceCollection services)
    {
        services.AddApplication(new Win32Application());
        return services;
    }
}