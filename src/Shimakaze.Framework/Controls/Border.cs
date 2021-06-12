using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.Controls
{
    public class Border : TextureBlock
    {
        #region Private Fields

        private Control? child;

        #endregion Private Fields

        #region Public Properties

        public bool AutoSize { get; set; } = true;

        public Color BorderColor { get; set; }

        public virtual Control? Child
        {
            get => child; set
            {
                child = value;
                if (child is not null)
                {
                    child.Parent = this;
                    child.Initialize();
                }
            }
        }

        public bool ForceSyncSize { get; set; } = true;

        public override Vector2 Size
        {
            get
            {
                if (Child is not null && AutoSize)
                    base.Size = Child.Size;
                return base.Size;
            }
            set
            {
                base.Size = value;
                AutoSize = false;
            }
        }

        public Vector4 Thickness { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public Border()
        {
        }

        public Border(Control child, bool forceSyncSize = true)
        {
            Child = child;
            ForceSyncSize = forceSyncSize;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void LoadContent()
        {
            base.LoadContent();

            if (Child is not null && ForceSyncSize)
                Child.Size = Size;
        }

        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(AutoSize):
                    AutoSize = StringConverter.ToBoolean(value);
                    return true;

                case nameof(ForceSyncSize):
                    ForceSyncSize = StringConverter.ToBoolean(value);
                    return true;

                case nameof(BorderColor):
                    BorderColor = StringConverter.ToColor(value);
                    return true;

                case nameof(Thickness):
                    Thickness = StringConverter.ToVector4(value);
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
                return;
            Graphics!.Begin();
            base.Draw(gameTime);

            Child?.Draw(gameTime);
            Graphics!.DrawRectangle(TruePosition.ToVector4(TrueSize), BorderColor, Thickness);
            Graphics!.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (Child is not null && Child.Size != Size)
                Size = Child!.Size;

            base.Update(gameTime);
            Child?.Update(gameTime);
        }

        #endregion Public Methods
    }
}