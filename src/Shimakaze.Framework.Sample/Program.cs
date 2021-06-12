using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;

using Shimakaze.Framework;
using Shimakaze.LogTracer;

namespace Shimakaze.Framework.Sample
{
    static class Program
    {

        static readonly string DLLFolder = Path.Combine("lib", "Native");

        static void Main(string[] args)
        {
            // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
            // NOTE: from documentation: 
            //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
            //           <key>NSHighResolutionCapable</key>
            //           <string>True</string>
            Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");
            Logger.Debug($"Application Location: {AppContext.BaseDirectory}");
            Directory.SetCurrentDirectory(Directory.GetParent(AppContext.BaseDirectory)!.FullName);

            NativeLibrary.SetDllImportResolver(typeof(Game).Assembly, DllImportResolver);
            NativeLibrary.SetDllImportResolver(typeof(Application).Assembly, DllImportResolver);


            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            using App game = new();

            bool isHighDPI = Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1";
            if (isHighDPI)
                Logger.Debug("HiDPI Enabled");

            game.Run();

        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Logger.Debug("Application Exit...");
        }

        static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            Logger.Debug($"[Native Invoke]::{libraryName}");
            IntPtr result;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Xbox:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                    if (Environment.Is64BitProcess)
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(DLLFolder, "x64", $"{libraryName}.dll"), out result))
                            return result;
                    }
                    else
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(DLLFolder, "x86", $"{libraryName}.dll"), out result))
                            return result;
                    }
                    break;
                case PlatformID.Other:
                case PlatformID.Unix:
                    if (NativeLibrary.TryLoad(Path.Combine(DLLFolder, "lib64", $"{libraryName}.so.0"), out result))
                        return result;
                    else if (NativeLibrary.TryLoad(Path.Combine(DLLFolder, "lib64", $"{libraryName}.so"), out result))
                        return result;
                    break;
                case PlatformID.MacOSX:
                    if (NativeLibrary.TryLoad(Path.Combine(DLLFolder, "osx", $"{libraryName}.dylib"), out result))
                        return result;
                    break;
            }
            return NativeLibrary.Load(libraryName);
        }

    }
}
