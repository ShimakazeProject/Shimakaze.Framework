// See https://aka.ms/new-console-template for more information

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;

Platform
    .CreateApplication("org.shimakaze.example")
    .Run(application => application
        .CreateWindow("Test Window", window => application
            .CreateWebView(window, webView => webView
                .Bind(i => i.Width, window, i => i.Width)
                .Bind(i => i.Height, window, i => i.Height)
                .NavigateTo("https://cn.bing.com")))
        .Show());
