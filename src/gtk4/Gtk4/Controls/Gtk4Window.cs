using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework.Gtk4.Controls;

public sealed class Gtk4Window : Window, IDisposable
{
    internal Gtk.Window Native { get; private set; }
    private bool _disposedValue;

    public Gtk4Window(string name) : base(name)
    {
        Native = Gtk.Window.New();
        if (Parent is Gtk4Window { Native: { } parent })
            Native.SetParent(parent);
        Native.SetTitle(Name);
        Native.SetDefaultSize(Width, Height);

        Native.OnShow += (_, _) => OnActivated();
        Native.OnCloseRequest += (_, _) =>
        {
            WindowCloseEventArgs args = new();
            OnClosing(args);
            return !args.CanClose;
        };
        Native.OnDestroy += (_, _) => OnClosed();

    }

    public override void Close() => Native.Close();

    public override void Show() => Native.Show();

    internal static Gtk4Window? FindWindow(UIElement? element) => element switch
    {
        null => null,
        Gtk4Window window => window,
        _ => FindWindow(element.Parent)
    };

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Native.Dispose();
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

    internal void AppendToApplication(Gtk.Application application) => application.AddWindow(Native);
}