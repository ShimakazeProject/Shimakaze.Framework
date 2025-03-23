using Shimakaze.Framework.Gtk4.Controls;
using Shimakaze.Framework.Win32.Controls;

namespace Shimakaze.Framework.Controls;
public static class WebViewUtils
{
    public static WebView CreateWebView(this Application application)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            return new EdgeWebView2();

        if (OperatingSystem.IsLinux())
            return new WebKitGtk6WebView();

        throw new PlatformNotSupportedException();
    }
    public static WebView CreateWebView(this Application application, Window window, Action<WebView> initialize)
    {
        var webView = CreateWebView(application);
        window.Content = webView;
        initialize(webView);
        return webView;
    }
}
