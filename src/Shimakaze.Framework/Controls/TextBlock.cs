using System;
using System.Text;

using FontStashSharp;

using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.Controls
{
    public class TextBlock : Control
    {
        #region Private Fields

        private DynamicSpriteFont? _fontCache;
        private string text = string.Empty;

        #endregion Private Fields

        #region Private Properties

        private DynamicSpriteFont FontCache
        {
            get
            {
                if (_fontCache is not null)
                    return _fontCache;
                return _fontCache = FontSystem!.GetFont(FontSize);
            }
        }

        #endregion Private Properties

        #region Public Properties

        public Color Color { get; set; } = Color.Black;

        public int FontSize { get; set; }

        public FontSystem? FontSystem { get; set; }

        public string? Text
        {
            get => text;
            set => text = value ?? string.Empty;
        }

        #endregion Public Properties

        #region Public Constructors

        public TextBlock()
        {
        }

        public TextBlock(string? text)
        {
            Text = text;
        }

        #endregion Public Constructors

        #region Private Methods

        private string GetText()
        {
            var size = FontCache.MeasureString(Text);
            if (size.X <= Size.X)// && size.Y <= Size.Y
                return text;

            StringBuilder sb = new(text);
            int p = sb.Length - 1;
            while (size.X > Size.X && p > 0)
            {
                p--;
                sb.Insert(p, Environment.NewLine);
                size = FontCache.MeasureString(sb);
            }
            return sb.Remove(p, sb.Length - p).ToString();
        }

        #endregion Private Methods

        #region Protected Methods

        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(Text):
                    Text = value;
                    return true;

                case nameof(FontSize):
                    FontSize = StringConverter.ToInt32(value);
                    return true;

                case nameof(FontSystem):
                    FontSystem = StringConverter.ToFontSystem(value);
                    return true;

                case nameof(Color):
                    Color = StringConverter.ToColor(value);
                    return true;

                default:
                    return false;
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
            {
                return;
            }
            base.Draw(gameTime);

            Graphics!.DrawString(FontCache, GetText(), TruePosition, Color);
        }

        #endregion Public Methods
    }
}