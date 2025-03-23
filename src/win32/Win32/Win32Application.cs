using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Win32.Controls;

using Windows.Win32;

namespace Shimakaze.Framework.Win32;

public sealed class Win32Application(Dispatcher dispatcher, string applicationId) : Application(dispatcher)
{
    private bool _isAppendedWindow = false;

    private readonly List<Win32Window> _windows = [];
    internal bool ShouldQuit => _isAppendedWindow && _windows.Count is 0;

    public override Window CreateWindow(string title)
    {
        Win32Window window = new(title);
        _windows.Add(window);
        _isAppendedWindow = true;
        window.Closed += Window_Closed;
        return window;
    }

    public override void Run() => Dispatcher.Run();

    public override void Stop() => PInvoke.PostQuitMessage(0);

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (sender is not Win32Window window)
            return;

        window.Closed -= Window_Closed;
        _windows.Remove(window);
    }

    internal new void OnInitialize() => base.OnInitialize();
}