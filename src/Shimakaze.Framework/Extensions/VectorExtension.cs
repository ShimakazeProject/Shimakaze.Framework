
using Microsoft.Xna.Framework;

namespace Shimakaze.Framework
{
    public static class VectorExtension
    {
        public static bool Contains(this Vector4 @this, Vector2 value)
        {
            return (@this.X <= value.X) &&
                   (value.X < (@this.X + @this.Z)) &&
                   (@this.Y <= value.Y) &&
                   (value.Y < (@this.Y + @this.W));
        }
        public static bool Intersects(this Vector4 @this, Vector4 value)
        {
            return (value.X < @this.Z &&
                    @this.X < value.Z &&
                    value.Y < @this.W &&
                    @this.Y < value.W);
        }

        public static Rectangle ToRectangle(this Vector4 @this)
        {
            return new((int)@this.X, (int)@this.Y, (int)@this.Z, (int)@this.W);
        }
        public static Vector4 ToVector4(this Rectangle @this)
        {
            return new(@this.X, @this.Y, @this.Width, @this.Height);
        }
        public static Vector4 ToVector4(this Vector2 @this, Vector2 size)
        {
            return new(@this.X, @this.Y, size.X, size.Y);
        }
    }
    //public static class PointExtension
    //{
    //    public static Vector2 ToVectory2(this Point point) => new(point.X, point.Y);
    //}
}
