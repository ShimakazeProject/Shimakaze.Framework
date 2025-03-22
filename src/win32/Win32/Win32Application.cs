using Shimakaze.Framework.Controls;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace Shimakaze.Framework.Win32;

public sealed class Win32Application : Application
{
    internal const uint WM_TASK = PInvoke.WM_USER + 1;

    private readonly List<Window> _windows = [];
    private readonly Win32Dispatcher _dispatcher;

    public Win32Application() : base()
    {
        _dispatcher = new(MainLoop);
        Dispatcher = _dispatcher;
    }

    public override void Run() => _dispatcher.Run();

    public override void Stop() => PInvoke.PostQuitMessage(0);

    private void MainLoop()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            _dispatcher.Initialize();
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(_dispatcher));

        OnInitialize();
        while (true)
        {
            if (!PInvoke.GetMessage(out var msg, HWND.Null, 0, 0))
                break;

            if (msg.message is WM_TASK)
                _dispatcher.ExecuteTask();

            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);

            if (_windows.Count is 0)
                PInvoke.PostQuitMessage(0);
        }
    }

    public override void AddWindow(Window window)
    {
        _windows.Add(window);
        window.Closed += Window_Closed;
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (sender is not Window window)
            return;

        _windows.Remove(window);
        window.Closed -= Window_Closed;
    }

}
