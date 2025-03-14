namespace Shimakaze.Framework;

public sealed class WindowCloseEventArgs : EventArgs
{
    public bool CanClose { get; set; } = true;
}