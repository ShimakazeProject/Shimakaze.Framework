// See https://aka.ms/new-console-template for more information

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Gtk4;
using Shimakaze.Framework.Gtk4.Controls;
using Shimakaze.Framework.Win32;
using Shimakaze.Framework.Win32.Controls;

static Application CreateApplication(string applicationId)
{
    if (OperatingSystem.IsWindowsVersionAtLeast(5))
        return new Win32Application();

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        return new Gtk4Application(applicationId);

    throw new PlatformNotSupportedException();
}

static Window CreateWindow(string title)
{
    if (OperatingSystem.IsWindowsVersionAtLeast(5))
        return new Win32Window(title);

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        return new Gtk4Window(title);

    throw new PlatformNotSupportedException();
}

static WebView CreateWebView()
{
    if (OperatingSystem.IsWindowsVersionAtLeast(5))
        return new EdgeWebView2();

    if (OperatingSystem.IsLinux())
        return new WebKitGtk6WebView();

    throw new PlatformNotSupportedException();
}

Application application = CreateApplication("org.shimakaze.test");

application.Initialize += (_, _) =>
{
    Window window = CreateWindow("Test Window");

    application.AddWindow(window);

    WebView webView = CreateWebView();

    window.Content = webView;

    webView.NavigateTo("https://cn.bing.com");

    window.Show();
};

application.Run();
