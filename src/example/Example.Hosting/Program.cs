// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Hosting;

Console.WriteLine("Hello, World!");

var builder = Host.CreateApplicationBuilder();
builder.Services
    .AddApplication("org.shimakaze.example.hosting")
    .UseShimakazeFramework((app, _) =>
    {
        if (app is not Application application)
            return;

        Window window = WindowUtils.CreateWindow("Test Window");

        application.AddWindow(window);

        WebView webView = WebViewUtils.CreateWebView();

        window.Content = webView;

        webView.NavigateTo("https://cn.bing.com");

        window.Show();
    });

var app = builder.Build();

app.Run();