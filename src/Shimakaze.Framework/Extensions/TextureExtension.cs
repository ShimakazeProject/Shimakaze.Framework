using Microsoft.Xna.Framework.Graphics;

using Shimakaze.Framework.Brushes;

namespace Shimakaze.Framework
{
    public static class TextureExtension
    {
        public static Brush ToBrush(this Texture2D @this, ImageMode mode = ImageMode.None) => new TextureBrush(@this, mode);
    }
}
