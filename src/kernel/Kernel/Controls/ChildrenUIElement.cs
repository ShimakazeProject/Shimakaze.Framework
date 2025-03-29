using System.Collections.Specialized;
using System.Diagnostics;

namespace Shimakaze.Framework.Controls;

public abstract class ChildrenUIElement : UIElement, INotifyCollectionChanged
{
    private readonly List<UIElement> _children = [];

    public IReadOnlyList<UIElement> Children => _children.AsReadOnly();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public void Add(UIElement child)
    {
        if (_children.Contains(child))
            return;

        Debug.Assert(child is { Parent: null });
        if (child is not { Parent: null })
            throw new InvalidOperationException();

        _children.Add(child);
        child.Parent = this;
        OnCollectionChanged(NotifyCollectionChangedAction.Add, child);
    }

    public void Remove(UIElement child)
    {
        if (!_children.Contains(child))
            return;

        child.Parent = null;
        _children.Remove(child);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, child);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, UIElement changedItem)
    {
        CollectionChanged?.Invoke(this, new(action, changedItem));
    }
}
