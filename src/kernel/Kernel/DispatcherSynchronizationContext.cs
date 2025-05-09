namespace Shimakaze.Framework;

public sealed class DispatcherSynchronizationContext(Dispatcher dispatcher) : SynchronizationContext
{
    public override void Send(SendOrPostCallback d, object? state)
    {
        if (dispatcher.CheckAccess())
            d(state);
        else
            dispatcher.Invoke(DispatcherPriority.Normal, () => d(state));
    }

    public override void Post(SendOrPostCallback d, object? state) => dispatcher.Invoke(DispatcherPriority.Normal, () => d(state));
}