using Shimakaze.Framework.Gtk4.Controls;
using Shimakaze.Framework.Win32.Controls;

namespace Shimakaze.Framework.Controls;
public static class WebViewUtils
{
    public static WebView CreateWebView()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5))
            return new EdgeWebView2();

        if (OperatingSystem.IsLinux())
            return new WebKitGtk6WebView();

        throw new PlatformNotSupportedException();
    }
}
