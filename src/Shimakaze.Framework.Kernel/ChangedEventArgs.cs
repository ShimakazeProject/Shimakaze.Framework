namespace Shimakaze.Framework;

public class ChangedEventArgs<T>(T old, T @new) : EventArgs
{
    public T Old { get; } = old;
    public T New { get; } = @new;
}