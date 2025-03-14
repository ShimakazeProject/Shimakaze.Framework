namespace Shimakaze.Framework.Controls;

public abstract partial class Window : ContentUIElement
{
    public new Window? Parent
    {
        get => base.Parent as Window;
        internal set => base.Parent = value;
    }

    protected Window(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        X = 0;
        Y = 0;
        Width = 0;
        Height = 0;
    }

    [PropertyChangedEvent]
    public virtual partial string Name { get; set; }

    [EventMethod]
    public event EventHandler? Activated;

    [EventMethod]
    public event EventHandler? Deactivated;

    [EventMethod]
    public event EventHandler<WindowCloseEventArgs>? Closing;

    [EventMethod]
    public event EventHandler? Closed;

    public abstract void Close();

    public abstract void Show();
}
