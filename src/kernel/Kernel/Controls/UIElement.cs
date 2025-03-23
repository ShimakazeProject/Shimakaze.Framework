namespace Shimakaze.Framework.Controls;

public abstract partial class UIElement
{
    [PropertyChangedEvent]
    public virtual partial UIElement? Parent { get; internal set; }

    [PropertyChangedEvent]
    public virtual partial int X { get; set; }

    [PropertyChangedEvent]
    public virtual partial int Y { get; set; }

    [PropertyChangedEvent]
    public virtual partial int Width { get; set; }

    [PropertyChangedEvent]
    public virtual partial int Height { get; set; }

    [PropertyChangedEvent]
    public virtual partial Visibility Visibility { get; set; }
}