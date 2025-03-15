using System.ComponentModel;
using System.Runtime.Versioning;

using Windows.Win32;

namespace Shimakaze.Framework.Win32;

[SupportedOSPlatform("windows5.0")]
public sealed class Win32Dispatcher : Dispatcher
{
    private uint _threadId = 0;

    public Win32Dispatcher(ThreadStart mainLoop) : base(mainLoop)
    {
        if (OperatingSystem.IsWindows())
            Thread.SetApartmentState(ApartmentState.STA);
    }

    public override void Invoke(Action action)
    {
        _tasks.Enqueue((action, null));
        if (!PInvoke.PostThreadMessage(_threadId, Win32Application.WM_TASK, 0, 0))
            throw new Win32Exception();
    }

    public override Task InvokeAsync(Action action)
    {
        TaskCompletionSource taskCompletionSource = new(0);
        _tasks.Enqueue((action, taskCompletionSource));
        if (!PInvoke.PostThreadMessage(_threadId, Win32Application.WM_TASK, 0, 0))
            throw new Win32Exception();

        return taskCompletionSource.Task;
    }

    internal void ExecuteTask()
    {
        if (!_tasks.TryDequeue(out var task))
            return;

        try
        {
            task.Action();
            task.Promise?.SetResult();
        }
        catch (Exception e)
        {
            task.Promise?.SetException(e);
        }
    }

    [SupportedOSPlatform("windows5.1.2600")]
    internal void Initialize()
    {
        _threadId = PInvoke.GetCurrentThreadId();
    }

    internal new void Run() => base.Run();
}
