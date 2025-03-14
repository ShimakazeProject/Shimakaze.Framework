namespace Shimakaze.Framework.Controls;

public abstract partial class ContentUIElement : UIElement
{
    [PropertyChangedEvent]
    public virtual UIElement? Content
    {
        get => field;
        set
        {
            if (field == value)
                return;
            var old = field;
            field = value;
            OnContentChanged(new(old, value));
        }
    }
}
