// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shimakaze.Framework;
using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Services.AddShimakazeFramework("org.shimakaze.example.hosting");

var app = builder.Build();

app.UseShimakazeFramework(application => application
    .CreateWindow("Test Window", window => application
        .CreateWebView(window, webView => webView
            .Bind(i => i.Width, window, i => i.Width)
            .Bind(i => i.Height, window, i => i.Height)
            .NavigateTo("https://cn.bing.com")))
    .Show()
);

app.Run();
