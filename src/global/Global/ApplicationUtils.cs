using Shimakaze.Framework.Gtk4;
using Shimakaze.Framework.Win32;

namespace Shimakaze.Framework;
public static class ApplicationUtils
{
    public static Application CreateApplication(string applicationId)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5))
            return new Win32Application();

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return new Gtk4Application(applicationId);

        throw new PlatformNotSupportedException();
    }
}
