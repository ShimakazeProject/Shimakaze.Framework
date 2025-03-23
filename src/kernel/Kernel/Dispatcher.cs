using System.Runtime.CompilerServices;
using System.Threading;

namespace Shimakaze.Framework;

/// <summary>
/// 调度器
/// </summary>
/// <param name="mainLoop"></param>
public abstract class Dispatcher
{
    private readonly Thread _thread;
    private readonly Lazy<DispatcherSynchronizationContext> _synchronizationContext;

    public DispatcherSynchronizationContext DispatcherSynchronizationContext => _synchronizationContext.Value;

    protected Dispatcher()
    {
        _synchronizationContext = new(() => new(this));
        _thread = new(MainLoop)
        {
            Name = "UI Thread",
            IsBackground = false,
        };

        if (OperatingSystem.IsWindows())
            _thread.SetApartmentState(ApartmentState.STA);
    }

    internal bool CheckAccess() => _thread == Thread.CurrentThread;

    protected abstract void Enqueue(DispatcherPriority priority, DispatchedHandler handler);

    protected virtual void MainLoop() => SynchronizationContext.SetSynchronizationContext(DispatcherSynchronizationContext);

    public void Run()
    {
        _thread.Start();
        _thread.Join();
    }

    public void Invoke(DispatcherPriority priority, Action action) => Enqueue(priority, action);

    public void Invoke(DispatcherPriority priority, Action<CancellationToken> action) => Enqueue(priority, action);

    public async Task<TResult> InvokeAsync<TResult>(DispatcherPriority priority, Func<TResult> func)
    {
        DispatchedHandler handler = new(Unsafe.As<Func<object>>(func));
        Enqueue(priority, handler);
        var result = await handler.ReturnableTask!;
        return (TResult)result;
    }

    public async Task<TResult> InvokeAsync<TResult>(DispatcherPriority priority, Func<CancellationToken, TResult> func)
    {
        DispatchedHandler handler = new(Unsafe.As<Func<CancellationToken, object>>(func));
        Enqueue(priority, handler);
        var result = await handler.ReturnableTask!;
        return (TResult)result;
    }
}
