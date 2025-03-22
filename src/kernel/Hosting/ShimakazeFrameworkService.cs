using Microsoft.Extensions.Hosting;

namespace Shimakaze.Framework.Hosting;

public sealed class ShimakazeFrameworkService(IHostApplicationLifetime lifetime, Application application, ShimakazeFrameworkServiceOptions options) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        application.Initialize += options.OnInitialize;

        _ = Task.Run(() =>
        {
            application.Run();
            lifetime.StopApplication();
        }, cancellationToken);

        cancellationToken.Register(application.Stop);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        application.Stop();

        return Task.CompletedTask;
    }
}
