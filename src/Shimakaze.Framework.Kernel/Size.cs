namespace Shimakaze.Framework;

public record struct Size(int Width, int Height)
{
    public static readonly Size Zero = default;
}
