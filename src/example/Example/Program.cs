// See https://aka.ms/new-console-template for more information

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;

Application application = ApplicationUtils.CreateApplication("org.shimakaze.test");

application.Initialize += (_, _) =>
{
    Window window = WindowUtils.CreateWindow("Test Window");

    application.AddWindow(window);

    WebView webView = WebViewUtils.CreateWebView();

    window.Content = webView;
    webView.Bind(i => i.Width, window, i => i.Width);
    webView.Bind(i => i.Height, window, i => i.Height);

    webView.NavigateTo("https://cn.bing.com");

    window.Show();
};

application.Run();
