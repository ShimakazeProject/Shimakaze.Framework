namespace Shimakaze.Framework.Gtk4;

public sealed class Gtk4Dispatcher(ThreadStart mainLoop) : Dispatcher(mainLoop)
{
    public override void Invoke(Action action)
    {
        GLib.Functions.IdleAdd(GLib.Constants.PRIORITY_DEFAULT, () =>
        {
            action();
            return false;
        });
    }

    public override Task InvokeAsync(Action action)
    {
        TaskCompletionSource taskCompletionSource = new();
        GLib.Functions.IdleAdd(GLib.Constants.PRIORITY_DEFAULT, () =>
        {
            try
            {
                action();
                taskCompletionSource.SetResult();
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return false;
        });
        return taskCompletionSource.Task;
    }

    internal new void Run() => base.Run();
}
