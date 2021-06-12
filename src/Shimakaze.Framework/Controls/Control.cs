using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Shimakaze.Framework.EventArguments;
using Shimakaze.LogTracer;

namespace Shimakaze.Framework.Controls
{
    public abstract class Control : IDisposable, INotifyPropertyChanged
    {
        #region Private Fields

        private const int DOUBLE_CLICK_DELAY = 500;
        private bool _initialized;
        private bool disposedValue;
        private bool enabled = true;
        private GraphicsManager? graphics;
        private bool isFocus;
        private Vector2 lastMousePosition;
        private bool visible = true;
        private bool waitSecondClick = false;

        #endregion Private Fields

        #region Public Fields

        public EventHandler? Click;

        public EventHandler? DoubleClick;

        public EventHandler<EventArgs>? EnabledChanged;

        public EventHandler? MouseEnter;

        public EventHandler? MouseLeave;

        public EventHandler? MouseLeftDown;

        public EventHandler? MouseLeftUp;

        public EventHandler<MouseMoveEventArgs>? MouseMove;

        public EventHandler? MouseRightDown;

        public EventHandler? MouseRightUp;

        public EventHandler? RightClick;

        public EventHandler<EventArgs>? VisibleChanged;

        #endregion Public Fields

        #region Public Constructors

        public Control()
        {
        }

        public Control(string? name)
        {
            Name = name;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Public Events

        #region Protected Properties

        protected virtual bool CanSetFocus => true;

        #endregion Protected Properties

        #region Protected Internal Properties

        protected internal GraphicsManager? Graphics
        {
            get
            {
                if (graphics is not null)
                    return graphics;
                else if (Parent is not null)
                    return graphics = Parent.Graphics;
                else if (Application.Current is not null)
                    return graphics = Application.Current.Graphics;
                else
                    throw new Exception("Cannot Found GraphicsManager");
            }
            internal set => graphics = value;
        }

        #endregion Protected Internal Properties

        #region Public Properties

        // TODO
        //public event EventHandler? MouseScrolled;
        public virtual bool Enabled
        {
            get => enabled; set
            {
                enabled = value;
                EnabledChanged?.Invoke(this, new());
            }
        }

        public virtual bool IsFocus
        {
            get => isFocus; set
            {
                if (!CanSetFocus)
                    return;
                isFocus = value;
                Application.Current!.Window?.SetCurrentFocusControl(this);
            }
        }

        public virtual bool IsHover { get; protected set; }

        public virtual bool IsMouseLeftDown { get; protected set; }

        public virtual bool IsMouseLeftUp { get; private set; }

        public virtual bool IsMouseRightDown { get; protected set; }

        public virtual bool IsMouseRightUp { get; private set; }

        public virtual Vector4 Margin { get; set; }

        public virtual string? Name { get; set; }

        public virtual Control? Parent { get; set; }

        public virtual Vector2 Position { get; set; } = new(0);

        public virtual Vector2 Size { get; set; } = new(100);

        public virtual bool Visible
        {
            get => visible; set
            {
                visible = value;
                VisibleChanged?.Invoke(this, new());
            }
        }

        public Vector2 TruePosition => (Parent?.TruePosition ?? new Vector2(0)) + Position + new Vector2(Margin.X, Margin.Y);

        public Vector2 TrueSize => Parent is not null && Parent.TrueSize.X < Size.X && Parent.TrueSize.Y < Size.Y
                    ? Parent.TrueSize - new Vector2(Margin.Z, Margin.W)
                    : Size - new Vector2(Margin.Z, Margin.W);

        #endregion Public Properties

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_initialized)
                    {
                        UnloadContent();
                    }
                }
                disposedValue = true;
            }
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void OnClick()
        {
            Click?.Invoke(this, new());
            IsFocus = true;
        }

        protected virtual void OnDoubleClick()
        {
            DoubleClick?.Invoke(this, new());
        }

        protected virtual void OnMouseEnter()
        {
            MouseEnter?.Invoke(this, new());
        }

        protected virtual void OnMouseLeave()
        {
            MouseLeave?.Invoke(this, new());
        }

        protected virtual void OnMouseLeftDown()
        {
            MouseLeftDown?.Invoke(this, new());
        }

        protected virtual void OnMouseLeftUp()
        {
            MouseLeftUp?.Invoke(this, new());
        }

        protected virtual void OnMouseMove(MouseMoveEventArgs arg)
        {
            MouseMove?.Invoke(this, arg);
        }

        protected virtual void OnMouseRightDown()
        {
            MouseRightDown?.Invoke(this, new());
        }

        protected virtual void OnMouseRightUp()
        {
            MouseRightUp?.Invoke(this, new());
        }

        protected virtual void OnRightClick()
        {
            RightClick?.Invoke(this, new());
        }

        protected virtual bool Parse(string property, string value)
        {
            switch (property)
            {
                case nameof(Margin):
                    Margin = StringConverter.ToVector4(value);
                    return true;

                case nameof(Name):
                    Name = value;
                    return true;

                case nameof(Position):
                    Position = StringConverter.ToVector2(value);
                    return true;

                case nameof(Size):
                    Size = StringConverter.ToVector2(value);
                    return true;

                case nameof(Visible):
                    Visible = StringConverter.ToBoolean(value);
                    return true;

                case nameof(Enabled):
                    Enabled = StringConverter.ToBoolean(value);
                    return true;

                default:
                    return false;
            }
        }

        protected virtual void UnloadContent()
        {
        }

        #endregion Protected Methods

        #region Public Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                LoadContent();
            }
        }

        public void Parse(Dictionary<string, string> properties) => Parallel.ForEach(properties, i =>
        {
            if (!Parse(i.Key, i.Value))
                Logger.Warn($"Property {i.Key} was Not Set");
        });

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            {
                bool left_down = IsHover && Input.LeftMouseIsDown;
                bool left_up = IsHover && Input.LeftMouseIsUp;
                bool right_down = IsHover && Input.RightMouseIsDown;
                bool right_up = IsHover && Input.RightMouseIsUp;

                if (IsMouseLeftDown && !left_down)
                {
                    if (waitSecondClick)
                    {
                        OnDoubleClick();
                        waitSecondClick = false;
                    }
                    else
                    {
                        OnClick();

                        new Action(async () =>
                        {
                            waitSecondClick = true;
                            await Task.Delay(DOUBLE_CLICK_DELAY);
                            waitSecondClick = false;
                        })();
                    }
                }
                if (IsMouseRightDown && !right_down)
                    OnRightClick();

                if (IsMouseLeftDown = left_down)
                    OnMouseLeftDown();
                else if (IsMouseLeftUp = left_up)
                    OnMouseLeftUp();

                if (IsMouseRightDown = right_down)
                    OnMouseRightDown();
                else if (IsMouseRightUp = right_up)
                    OnMouseRightUp();

                if (IsHover)
                {
                    var mousePosition = Input.MousePosition;
                    if (lastMousePosition != mousePosition)
                    {
                        lastMousePosition = mousePosition;
                        OnMouseMove(new(mousePosition, lastMousePosition));
                    }
                }

                bool hover = TruePosition.ToVector4(TrueSize).Contains(Input.MousePosition);

                if (hover == IsHover)
                    return;
                else if (!IsHover && hover)
                    OnMouseEnter();
                else if (IsHover && !hover)
                    OnMouseLeave();

                IsHover = hover;
            }
        }

        #endregion Public Methods
    }
}