using System.Diagnostics;

using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Gtk4.Controls;

namespace Shimakaze.Framework.Gtk4;

public sealed class Gtk4Application : Application, IDisposable
{
    private readonly Gtk.Application _native;
    private bool _disposedValue;

    public Gtk4Application(Dispatcher dispatcher, string? applicationId) : base(dispatcher)
    {
        Gtk.Module.Initialize();
        _native = Gtk.Application.New(applicationId, Gio.ApplicationFlags.DefaultFlags);
        _native.OnActivate += (_, _) => OnInitialize();
    }

    internal void MainLoop()
    {
        var args = Environment.GetCommandLineArgs();
        var result = _native.Run(args.Length, args);
        Debug.Assert(result is 0);
    }

    public override void Run() => Dispatcher.Run();

    public override void Stop() => _native.Quit();

    public override Window CreateWindow(string title)
    {
        Gtk4Window window = new(title);

        window.AppendToApplication(_native);

        return window;
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
