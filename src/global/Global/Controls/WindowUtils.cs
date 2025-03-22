using Shimakaze.Framework.Gtk4.Controls;
using Shimakaze.Framework.Win32.Controls;

namespace Shimakaze.Framework.Controls;
public static class WindowUtils
{
    public static Window CreateWindow(string title)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5))
            return new Win32Window(title);

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return new Gtk4Window(title);

        throw new PlatformNotSupportedException();
    }
}
