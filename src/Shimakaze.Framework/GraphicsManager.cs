using Microsoft.Xna.Framework.Graphics;

namespace Shimakaze.Framework
{
    public class GraphicsManager
    {
        internal SpriteBatch SpriteBatch { get; }

        public GraphicsManager(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new(graphicsDevice);
        }
    }
}