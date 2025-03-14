using System.Runtime.Versioning;

using Shimakaze.Framework.Controls;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace Shimakaze.Framework.Win32;

[SupportedOSPlatform("windows5.0")]
public sealed class Win32Application : Application
{
    private readonly List<Window> _windows = [];

    public override void Stop()
    {
        PInvoke.PostQuitMessage(0);
    }

    public override void MainLoop()
    {
        OnInitialize();
        while (true)
        {
            if (!PInvoke.GetMessage(out var msg, HWND.Null, 0, 0))
                break;

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
