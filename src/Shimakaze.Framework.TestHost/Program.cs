// See https://aka.ms/new-console-template for more information

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Gtk4;
using Shimakaze.Framework.Gtk4.Controls;
using Shimakaze.Framework.Win32;
using Shimakaze.Framework.Win32.Controls;

Application application = OperatingSystem.IsWindowsVersionAtLeast(5)
    ? new Win32Application()
    : new Gtk4Application("org.shimakaze.test");

application.Initialize += (_, _) =>
{
    Window window = OperatingSystem.IsWindowsVersionAtLeast(5)
        ? new Win32Window("Test Window")
        : new Gtk4Window("Test Window");

    application.AddWindow(window);

    window.Show();
};

application.MainLoop();
