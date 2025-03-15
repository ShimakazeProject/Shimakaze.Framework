using System.Collections.Concurrent;

namespace Shimakaze.Framework;

public abstract class Dispatcher(ThreadStart mainLoop)
{
    protected readonly ConcurrentQueue<(Action Action, TaskCompletionSource? Promise)> _tasks = [];
    public Thread Thread { get; } = new(mainLoop)
    {
        Name = "UI Thread",
        IsBackground = false,
    };

    public abstract void Invoke(Action action);
    public abstract Task InvokeAsync(Action action);
    internal bool CheckAccess() => Thread == Thread.CurrentThread;

    protected void Run()
    {
        Thread.Start();
        Thread.Join();
    }
}