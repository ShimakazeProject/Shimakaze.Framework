namespace Shimakaze.Framework.Controls;

public abstract class ChildrenUIElement : UIElement
{
    public List<UIElement>? Children { get; init; } = [];
}
