using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework.Gtk4.Controls;

[UnsupportedOSPlatform("OSX")]
[UnsupportedOSPlatform("Windows")]
public sealed class WebKitGtk6WebView : WebView
{
    private readonly WebKit.WebView _native;
    public WebKitGtk6WebView()
    {
        WebKit.Module.Initialize();
        _native = WebKit.WebView.New();
    }

    public override void NavigateTo([StringSyntax("Uri")] string uri) => _native.LoadUri(uri);

    protected override void OnParentChanged(ChangedEventArgs<UIElement?> eventArgs)
    {
        base.OnParentChanged(eventArgs);
        if (Gtk4Window.FindWindow(Parent) is not { } window)
            return;

        window.Native.Child = _native;
    }
}
