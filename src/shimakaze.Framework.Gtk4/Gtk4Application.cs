using System.Diagnostics;

using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Gtk4.Controls;

namespace Shimakaze.Framework.Gtk4;

public sealed class Gtk4Application : Application, IDisposable
{
    private readonly Gtk.Application _native;
    private bool _disposedValue;

    public Gtk4Application(string? applicationId)
    {
        _native = Gtk.Application.New(applicationId, Gio.ApplicationFlags.DefaultFlags);
        _native.OnActivate += (_, _) => OnInitialize();
    }

    public override void MainLoop()
    {
        var args = Environment.GetCommandLineArgs();
        var result = _native.Run(args.Length, args);
        Debug.Assert(result is 0);
    }

    public override void Stop()
    {
        _native.Quit();
    }

    public override void AddWindow(Window window)
    {
        if (window is not Gtk4Window win)
            throw new InvalidCastException();

        win.AddToApplication(_native);
    }


    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _native.Dispose();
            }

            _disposedValue = true;
        }
    }

    // ~Gtk4Application()
    // {
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
