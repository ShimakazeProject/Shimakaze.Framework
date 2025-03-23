namespace Shimakaze.Framework.Gtk4;

public sealed class Gtk4Dispatcher : Dispatcher
{
    protected override void Enqueue(DispatcherPriority priority, DispatchedHandler handler)
    {
        GLib.Functions.IdleAdd(
            priority switch
            {
                DispatcherPriority.Idle => GLib.Constants.PRIORITY_DEFAULT_IDLE,
                DispatcherPriority.Low => GLib.Constants.PRIORITY_LOW,
                DispatcherPriority.Normal => GLib.Constants.PRIORITY_DEFAULT,
                DispatcherPriority.High => GLib.Constants.PRIORITY_HIGH,
                _ => GLib.Constants.PRIORITY_DEFAULT,
            },
            () =>
        {
            handler.Invoke();
            return false;
        });
    }

    protected override void MainLoop()
    {
        base.MainLoop();
        if (Application.Instance is not Gtk4Application app)
            throw new InvalidOperationException();

        app.MainLoop();
    }
}