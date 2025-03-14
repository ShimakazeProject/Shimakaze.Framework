using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Shimakaze.Framework.Controls;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Shimakaze.Framework.Win32.Controls;

[SupportedOSPlatform("windows5.0")]
public sealed class Win32Window : Window, IDisposable
{
    private static readonly Lazy<HINSTANCE> HInstance = new(() => (HINSTANCE)Process.GetCurrentProcess().Handle);

    private readonly WNDCLASSW _wndClass;
    private readonly HWND _hWnd;
    private bool _disposedValue;

    public string ClassName { get; }

    public unsafe Win32Window(string name)
        : base(name)
    {
        ClassName = Name[..Math.Min(255, Name.Length)];

        fixed (char* pClassName = ClassName)
        fixed (char* pName = Name)
        {
            _wndClass = new()
            {
                lpfnWndProc = WndProc,
                hInstance = HInstance.Value,
                lpszClassName = pClassName,
                style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
            };
            PInvoke.RegisterClass(_wndClass);

            _hWnd = PInvoke.CreateWindowEx(
                WINDOW_EX_STYLE.WS_EX_LEFT,
                pClassName,
                pName,
                WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                X is 0 ? PInvoke.CW_USEDEFAULT : X,
                Y is 0 ? PInvoke.CW_USEDEFAULT : Y,
                Width is 0 ? PInvoke.CW_USEDEFAULT : Width,
                Height is 0 ? PInvoke.CW_USEDEFAULT : Height,
                (Parent as Win32Window)?._hWnd ?? HWND.Null,
                HMENU.Null,
                HInstance.Value,
                null
            );
            if (_hWnd.IsNull)
                throw new Win32Exception();

            if (OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
            {
                BOOL value = true;
                PInvoke.DwmSetWindowAttribute(_hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &value, (uint)Marshal.SizeOf(value));
            }
        }
    }

    private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_CREATE:
                OnInitialize();
                break;
            case PInvoke.WM_PAINT:
                break;
            case PInvoke.WM_SHOWWINDOW:
                OnActivated();
                break;
            case PInvoke.WM_SIZE:
                break;
            case PInvoke.WM_CLOSE:
                WindowCloseEventArgs args = new();
                OnClosing(args);
                if (args.CanClose)
                    PInvoke.DestroyWindow(_hWnd);
                break;
            case PInvoke.WM_DESTROY:
                OnClosed();
                break;
        }
        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public override void Show()
    {
        PInvoke.ShowWindow(_hWnd, SHOW_WINDOW_CMD.SW_SHOWDEFAULT);
        PInvoke.UpdateWindow(_hWnd);
    }

    public override void Close() => PInvoke.CloseWindow(_hWnd);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }

            PInvoke.DestroyWindow(_hWnd);
            PInvoke.UnregisterClass(ClassName, Process.GetCurrentProcess().SafeHandle);
            _disposedValue = true;
        }
    }

    ~Win32Window()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}