using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shimakaze.Framework.Brushes
{
    public class TextureBrush : Brush
    {
        #region Private Fields

        private readonly Texture2D _texture;

        #endregion Private Fields

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _texture.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Protected Internal Methods

        protected internal override void Draw(GraphicsManager graphics, ref Vector2 position, ref Vector2 size)
        {
            Vector4 rect = position.ToVector4(size), rectangle = rect;
            switch (Mode)
            {
                case ImageMode.None:
                    rectangle = rect;
                    break;

                case ImageMode.Fill:
                    rectangle = _texture.Bounds.ToVector4();
                    break;

                case ImageMode.Uniform:
                case ImageMode.UniformFill:
                    (float height, float top) w;
                    (float width, float left) h;

                    // 计算宽度固定时的高度
                    {
                        var w_w = size.Y / size.X;
                        w.height = (float)Math.Round(_texture.Width * w_w);
                        w.top = (float)Math.Round((_texture.Height / 2) - (w.height / 2));
                    }

                    // 计算高度固定时的宽度
                    {
                        var h_h = size.X / size.Y;
                        h.width = (float)Math.Round(_texture.Height * h_h);
                        h.left = (float)Math.Round((_texture.Width / 2) - (h.width / 2));
                    }

                    if (Mode is ImageMode.Uniform)
                    {
                        if (w.height <= size.Y)
                            rectangle = new(0, w.top, _texture.Width, w.height);
                        else if (h.width >= size.X)
                            rectangle = new(h.left, 0, h.width, _texture.Height);
                        else
                            throw new Exception();
                    }
                    else
                    {
                        if (w.height >= size.Y)
                            rectangle = new(0, w.top, _texture.Width, w.height);
                        else if (h.width <= size.X)
                            rectangle = new(h.left, 0, h.width, _texture.Height);
                        else
                            throw new Exception();
                    }
                    break;
            }
            graphics.Draw(_texture, rect, rectangle);
        }

        #endregion Protected Internal Methods

        #region Public Constructors

        internal TextureBrush(Texture2D texture, ImageMode mode)
        {
            _texture = texture;
            Mode = mode;
        }

        #endregion Public Constructors

        #region Public Properties

        public ImageMode Mode { get; set; }

        #endregion Public Properties
    }
}