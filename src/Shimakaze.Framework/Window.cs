using Microsoft.Xna.Framework;

using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework
{
    public class Window : Control
    {
        #region Private Fields

        private Control? child;

        #endregion Private Fields

        #region Public Properties

        public Control? Child
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

        public Control? CurrentFocusControl { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Draw(gameTime);
            Child?.Draw(gameTime);
        }

        public void SetCurrentFocusControl(Control? value)
        {
            if (CurrentFocusControl == value)
                return;

            if (CurrentFocusControl is not null)
                CurrentFocusControl.IsFocus = false;

            CurrentFocusControl = value;

            if (CurrentFocusControl is not null)
                CurrentFocusControl.IsFocus = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);

            if (Child is not null)
            {
                Child.Position = Position;
                Child.Size = Size;
            }
            Child?.Update(gameTime);
        }

        #endregion Public Methods
    }
}