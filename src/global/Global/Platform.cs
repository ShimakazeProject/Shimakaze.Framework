using Shimakaze.Framework.Gtk4;
using Shimakaze.Framework.Win32;

namespace Shimakaze.Framework;
public static class Platform
{
    public static Dispatcher CreateDispatcher()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5))
            return new Win32Dispatcher();

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return new Gtk4Dispatcher();

        throw new PlatformNotSupportedException();
    }

    public static Application CreateApplication(Dispatcher dispatcher, string applicationId)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5))
            return new Win32Application(dispatcher, applicationId);

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return new Gtk4Application(dispatcher, applicationId);

        throw new PlatformNotSupportedException();
    }

    public static Application CreateApplication(string applicationId)
    {
        return CreateApplication(CreateDispatcher(), applicationId);
    }
}
