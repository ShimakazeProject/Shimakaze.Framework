using Microsoft.Xna.Framework;

using Shimakaze.Framework.Brushes;

namespace Shimakaze.Framework.Controls
{
    public class TextureBlock : Control
    {
        #region Public Properties

        public Brush? Texture { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(Texture):
                    Texture = StringConverter.ToBrush(value);
                    return true;

                default:
                    return false;
            }
        }

        protected override void UnloadContent()
        {
            Texture?.Dispose();
        }

        #endregion Protected Methods

        #region Public Methods

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;
            var position = TruePosition;
            var size = TrueSize;
            Texture?.Draw(Graphics!, ref position, ref size);
        }

        #endregion Public Methods
    }
}