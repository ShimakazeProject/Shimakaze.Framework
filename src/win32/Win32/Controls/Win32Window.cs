using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Shimakaze.Framework.Controls;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Shimakaze.Framework.Win32.Controls;

public sealed class Win32Window : Window, IDisposable
{
    private static readonly Lazy<HINSTANCE> HInstance = new(() => (HINSTANCE)Process.GetCurrentProcess().Handle);

    private readonly WNDCLASSW _wndClass;
    internal HWND HWND { get; private set; }
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

            HWND = PInvoke.CreateWindowEx(
                WINDOW_EX_STYLE.WS_EX_LEFT,
                pClassName,
                pName,
                WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                X is 0 ? PInvoke.CW_USEDEFAULT : X,
                Y is 0 ? PInvoke.CW_USEDEFAULT : Y,
                Width is 0 ? PInvoke.CW_USEDEFAULT : Width,
                Height is 0 ? PInvoke.CW_USEDEFAULT : Height,
                (Parent as Win32Window)?.HWND ?? HWND.Null,
                HMENU.Null,
                HInstance.Value,
                null
            );
            if (HWND.IsNull)
                throw new Win32Exception();

            if (!PInvoke.GetWindowRect(HWND, out var rect))
                throw new Win32Exception();

            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;

            XChanged += Win32Window_SizeChanged;
            YChanged += Win32Window_SizeChanged;
            WidthChanged += Win32Window_SizeChanged;
            HeightChanged += Win32Window_SizeChanged;

            if (OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
            {
                BOOL value = true;
                PInvoke.DwmSetWindowAttribute(HWND, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &value, (uint)Marshal.SizeOf(value));
            }
        }
    }

    private void Win32Window_SizeChanged(object? sender, ChangedEventArgs<int> e)
    {
        PInvoke.SetWindowPos(HWND, HWND.Null, X, Y, Width, Height, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }

    private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_CREATE:
                break;
            case PInvoke.WM_PAINT:
                break;
            case PInvoke.WM_SHOWWINDOW:
                OnActivated();
                break;
            case PInvoke.WM_SIZE:
                if (!PInvoke.GetWindowRect(HWND, out var rect))
                    throw new Win32Exception();

                XChanged -= Win32Window_SizeChanged;
                X = rect.X;
                XChanged += Win32Window_SizeChanged;

                YChanged -= Win32Window_SizeChanged;
                Y = rect.Y;
                YChanged += Win32Window_SizeChanged;

                WidthChanged -= Win32Window_SizeChanged;
                Width = rect.Width;
                WidthChanged += Win32Window_SizeChanged;

                HeightChanged -= Win32Window_SizeChanged;
                Height = rect.Height;
                HeightChanged += Win32Window_SizeChanged;

                break;
            case PInvoke.WM_CLOSE:
                WindowCloseEventArgs args = new();
                OnClosing(args);
                if (args.CanClose)
                    PInvoke.DestroyWindow(HWND);
                break;
            case PInvoke.WM_DESTROY:
                OnClosed();
                break;
        }
        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public override void Show()
    {
        PInvoke.ShowWindow(HWND, SHOW_WINDOW_CMD.SW_SHOWDEFAULT);
        PInvoke.UpdateWindow(HWND);
    }

    public override void Close() => PInvoke.CloseWindow(HWND);

    internal static Win32Window? FindWindow(UIElement? element) => element switch
    {
        null => null,
        Win32Window window => window,
        _ => FindWindow(element.Parent)
    };

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }

            PInvoke.DestroyWindow(HWND);
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