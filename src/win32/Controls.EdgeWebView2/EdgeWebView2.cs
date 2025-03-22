using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Microsoft.Web.WebView2.Core;

using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework.Win32.Controls;

public sealed class EdgeWebView2 : WebView
{
    private static CoreWebView2Environment? s_environment;
    private static Task? s_initializeTask;
    private CoreWebView2Controller? _controller;
    private TaskCompletionSource _initializeTaskCompletionSource = new();

    public EdgeWebView2()
    {
        if (s_environment is null)
            s_initializeTask = CoreWebView2Environment
                .CreateAsync(
                    browserExecutableFolder: null,
                    userDataFolder: Path.Combine(AppContext.BaseDirectory, "WebView2Data"),
                    options: new())
                .ContinueWith(async task =>
                {
                    s_environment = await task;
                });
    }

    private async void EdgeWebView2_SizeChanged(object? sender, ChangedEventArgs<int> e)
    {
        await _initializeTaskCompletionSource.Task;
        Debug.Assert(_controller is not null);
        _controller.Bounds = new(X, Y, Width, Height);
    }

    public override async void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri)
    {
        await _initializeTaskCompletionSource.Task;
        Debug.Assert(_controller is not null);
        _controller.CoreWebView2.Navigate(uri);
    }

    protected override async void OnParentChanged(ChangedEventArgs<UIElement?> eventArgs)
    {
        base.OnParentChanged(eventArgs);

        XChanged -= EdgeWebView2_SizeChanged;
        YChanged -= EdgeWebView2_SizeChanged;
        WidthChanged -= EdgeWebView2_SizeChanged;
        HeightChanged -= EdgeWebView2_SizeChanged;
        try
        {
            if (eventArgs.Old is null)
                _initializeTaskCompletionSource = new();

            _controller?.Close();

            if (Win32Window.FindWindow(Parent) is not { } window)
                return;

            Debug.Assert(s_initializeTask is not null);
            await s_initializeTask;

            Debug.Assert(s_environment is not null);
            _controller = await s_environment.CreateCoreWebView2ControllerAsync(window.HWND);
            if (Width is 0)
                Width = window.Width;
            if (Height is 0)
                Height = window.Height;
            _controller.Bounds = new(X, Y, Width, Height);

            _initializeTaskCompletionSource.SetResult();
        }
        catch (Exception ex)
        {
            _initializeTaskCompletionSource.SetException(ex);
        }
        finally
        {
            XChanged += EdgeWebView2_SizeChanged;
            YChanged += EdgeWebView2_SizeChanged;
            WidthChanged += EdgeWebView2_SizeChanged;
            HeightChanged += EdgeWebView2_SizeChanged;
        }
    }
}
