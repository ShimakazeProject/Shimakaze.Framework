using System.Text;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Shimakaze.Framework.Brushes;

namespace Shimakaze.Framework
{
    public static class GraphicsManagerExtension
    {
        #region Private Fields

        private static bool _isBegin;
        private static object _lock = new();
        private static Texture2D? whitePixelTexture;

        #endregion Private Fields

        #region Private Methods

        private static void Initialize(this GraphicsManager @this)
        {
            if (whitePixelTexture is null)
                whitePixelTexture = @this.CreateTexture2D(Color.White, 1, 1);
        }

        #endregion Private Methods

        #region Public Methods

        public static void Begin(this GraphicsManager @this)
        {
            lock (_lock)
            {
                if (_isBegin)
                    return;
                _isBegin = true;

                @this.SpriteBatch.Begin();
            }
        }

        public static Brush CreateBrush(this GraphicsManager @this, Color color, int width, int height) => @this.CreateTexture2D(color, width, height).ToBrush();

        public static Texture2D CreateTexture2D(this GraphicsManager @this, Color color, int width, int height)
        {
            Texture2D texture = new Texture2D(@this.SpriteBatch.GraphicsDevice, width, height, false, SurfaceFormat.Color);

            Color[] colorArray = new Color[width * height];

            for (int i = 0; i < colorArray.Length; i++)
                colorArray[i] = color;

            texture.SetData(colorArray);

            return texture;
        }

        public static void Draw(this GraphicsManager @this, Texture2D texture, Vector4 destinationRectangle)
        {
            @this.Begin();
            @this.SpriteBatch.Draw(texture, destinationRectangle.ToRectangle(), Color.White);
            @this.End();
        }

        public static void Draw(this GraphicsManager @this, Texture2D texture, Vector2 position)
        {
            @this.Begin();
            @this.SpriteBatch.Draw(texture, position, Color.White);
            @this.End();
        }

        public static void Draw(this GraphicsManager @this, Texture2D texture, Vector4 destinationRectangle, Vector4 sourceRectangle)
        {
            @this.Begin();
            @this.SpriteBatch.Draw(texture, destinationRectangle.ToRectangle(), sourceRectangle.ToRectangle(), Color.White);
            @this.End();
        }

        public static void DrawFillRectangle(this GraphicsManager @this, Vector4 rect, Color color)
        {
            @this.Initialize();
            @this.Begin();
            @this.SpriteBatch.Draw(whitePixelTexture, new Vector4(rect.X, rect.Y, rect.Z, rect.W).ToRectangle(), color);
            @this.End();
        }

        public static void DrawFillRectangle(this GraphicsManager @this, Vector4 rect, Vector4 sourceRectangle, Color color)
        {
            @this.Initialize();
            @this.Begin();
            @this.SpriteBatch.Draw(whitePixelTexture, new Vector4(rect.X, rect.Y, rect.Z, rect.W).ToRectangle(), sourceRectangle.ToRectangle(), color);
            @this.End();
        }

        public static void DrawRectangle(this GraphicsManager @this, Vector4 rect, Color color, Vector4 thickness)
        {
            @this.Begin();
            // Top
            @this.DrawFillRectangle(new(rect.X, rect.Y, rect.Z, thickness.Y), color);
            // Left
            @this.DrawFillRectangle(new(rect.X, rect.Y + thickness.Y, thickness.X, rect.W - thickness.Y), color);
            // Right
            @this.DrawFillRectangle(new(rect.X + rect.Z - thickness.X, rect.Y, thickness.Z, rect.W), color);
            // Bottom
            @this.DrawFillRectangle(new(rect.X, rect.Y + rect.W - thickness.Y, rect.Z, thickness.Z), color);
            @this.End();
        }

        public static void DrawString(this GraphicsManager @this, DynamicSpriteFont font, string text, Vector2 position, Color color)
        {
            @this.Begin();
            @this.SpriteBatch.DrawString(font, text, position, color);
            @this.End();
        }

        public static void DrawString(this GraphicsManager @this, DynamicSpriteFont font, StringBuilder text, Vector2 position, Color color)
        {
            @this.Begin();
            @this.SpriteBatch.DrawString(font, text, position, color);
            @this.End();
        }

        public static void End(this GraphicsManager @this)
        {
            lock (_lock)
            {
                if (!_isBegin)
                    return;
                _isBegin = false;

                @this.SpriteBatch.End();
            }
        }

        #endregion Public Methods
    }
}