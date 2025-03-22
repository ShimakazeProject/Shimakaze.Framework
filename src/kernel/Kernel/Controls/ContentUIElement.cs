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
            if (field is not null)
                field.Parent = null;
            field = value;
            if (field is not null)
                field.Parent = this;
            OnContentChanged(new(old, value));
        }
    }
}
