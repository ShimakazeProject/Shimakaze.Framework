using System.Runtime.Versioning;

using Shimakaze.Framework.Gtk4;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static IServiceCollection AddGtk4Application(this IServiceCollection services, string applicationId)
    {
        services.AddApplication(new Gtk4Application(applicationId));
        return services;
    }
}