using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shimakaze.Framework.Hosting;

public static class ServiceCollectionExtensions
{
    public static void UseShimakazeFramework(this IHost app, Action<Application> initialize)
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Application>>();
            var application = app.Services.GetRequiredService<Application>();
            _ = Task.Run(() =>
            {
                logger.LogInformation("Starting main message loop...");
                application.Run(initialize);
                logger.LogInformation("Main message loop exited.");
                lifetime.StopApplication();
            }).ConfigureAwait(false);
        });
    }
}