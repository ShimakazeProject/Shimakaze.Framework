using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.Controls
{
    public class TitleBar : TextureBlock
    {
        #region Public Constructors

        public TitleBar()
        {
            Icon = new()
            {
                Name = nameof(Icon),
                Parent = this,
            };
            Title = new()
            {
                Name = nameof(Title),
                Parent = this,
            };
            MaxButton = new()
            {
                Name = nameof(MaxButton),
                Parent = this,
            };
            MinButton = new()
            {
                Name = nameof(MinButton),
                Parent = this,
            };
            CloseButton = new()
            {
                Name = nameof(CloseButton),
                Parent = this,
            };
            Size = new(0, 32);
        }

        #endregion Public Constructors

        #region Public Properties

        public Button CloseButton { get; }

        public TextureBlock Icon { get; }

        public Button MaxButton { get; }

        public Button MinButton { get; }

        public TextBlock Title { get; }

        #endregion Public Properties

        #region Public Methods

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Draw(gameTime);

            Icon.Draw(gameTime);
            Title.Draw(gameTime);
            MaxButton.Draw(gameTime);
            MinButton.Draw(gameTime);
            CloseButton.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);
            Icon.Size = new(24);
            Title.Size = new(Size.X - Icon.Size.X - MaxButton.Size.X - MinButton.Size.X - CloseButton.Size.X - 32, Size.Y);
            MaxButton.Size = new(Size.X - Icon.Size.X - Title.Size.X - MinButton.Size.X - CloseButton.Size.X - 32, Size.Y);
            MinButton.Size = new(Size.X - Icon.Size.X - MaxButton.Size.X - Title.Size.X - CloseButton.Size.X - 32, Size.Y);
            CloseButton.Size = new(Size.X - Icon.Size.X - MaxButton.Size.X - MinButton.Size.X - Title.Size.X - 32, Size.Y);

            MinButton.Position = new Vector2(Size.X - MinButton.Size.X, 0);
            MaxButton.Position = MinButton.Position + new Vector2(32, 0);
            CloseButton.Position = MaxButton.Position + new Vector2(32, 0);
        }

        #endregion Public Methods
    }
}