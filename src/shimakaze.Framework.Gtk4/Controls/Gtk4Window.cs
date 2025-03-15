using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework.Gtk4.Controls;

public sealed class Gtk4Window : Window, IDisposable
{
    private readonly Gtk.Window _native;
    private bool _disposedValue;

    public Gtk4Window(string name) : base(name)
    {
        _native = Gtk.Window.New();
        if (Parent is Gtk4Window { _native: { } parent })
            _native.SetParent(parent);
        _native.SetTitle(Name);
        _native.SetDefaultSize(Width, Height);

        _native.OnActivateDefault += (_, _) => OnInitialize();
        _native.OnShow += (_, _) => OnActivated();
        _native.OnCloseRequest += (_, _) =>
        {
            WindowCloseEventArgs args = new();
            OnClosing(args);
            return !args.CanClose;
        };
        _native.OnDestroy += (_, _) => OnClosed();

    }

    public override void Close() => _native.Close();

    public override void Show() => _native.Show();

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

    // ~Gtk4Window()
    // {
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    internal void AddToApplication(Gtk.Application application) => application.AddWindow(_native);
}