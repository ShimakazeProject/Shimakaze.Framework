namespace Shimakaze.Framework;

public record struct Point(int X, int Y)
{
    public static readonly Point Zero = default;
}
