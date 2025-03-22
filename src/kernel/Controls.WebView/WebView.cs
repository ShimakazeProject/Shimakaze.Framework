using System.Diagnostics.CodeAnalysis;

namespace Shimakaze.Framework.Controls;

public abstract class WebView : UIElement
{
    public abstract void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri);
}
