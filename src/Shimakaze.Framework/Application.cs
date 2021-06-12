using System;
using System.ComponentModel;

using FontStashSharp;

using Microsoft.Xna.Framework;

using SDL2;

using Shimakaze.LogTracer;

namespace Shimakaze.Framework
{
    public class Application : Game, INotifyPropertyChanged
    {
        #region Private Fields

        private static Application? current;
        private Window? window;

        #endregion Private Fields

        #region Public Constructors

        public Application() : base()
        {
            FNALoggerEXT.LogError += Logger.Error;
            FNALoggerEXT.LogInfo += Logger.Info;
            FNALoggerEXT.LogWarn += Logger.Warn;

            Current = this;
            GraphicsDeviceManager = new(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = true,
            };
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Public Events

        #region Protected Properties

        protected GameWindow GameWindow => base.Window;

        #endregion Protected Properties

        #region Public Properties

        public static Application? Current
        {
            get => current; private set
            {
                if (current is not null)
                    throw new Exception("Repeat Create Application");
                current = value;
            }
        }

        public PropertyChangedEventHandler AddPropertyChanged { set => PropertyChanged += value; }

        public virtual bool Enabled { get; protected set; }

        public virtual FontSystem? FontSystem { get; protected set; }

        public virtual GraphicsManager? Graphics { get; private set; }

        public GraphicsDeviceManager? GraphicsDeviceManager { get; protected set; }

        public new Window? Window
        {
            get => window; set
            {
                window = value;
                if (window is not null)
                    window.Graphics = Graphics;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice!.Clear(Color.Transparent);
            window?.Draw(gameTime);
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (Window is not null)
            {
                Window.Initialize();
                GameWindow.ClientSizeChanged += (_, _) => Window.Size = new(GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
                Window.Size = new(GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            }
        }

        protected override void LoadContent()
        {
            Graphics = new(GraphicsDevice);
            FontSystem = FontSystemFactory.Create(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Content?.Unload();
            Window?.Dispose();
            Logger.Debug("Application UnloadContent");
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update(IsActive);
            Window?.Update(gameTime);
            FrameworkDispatcher.Update();
        }

        #endregion Protected Methods

        #region Public Methods

        public virtual void SetTextInputRect(ref Rectangle rect)
        {
            SDL.SDL_Rect sdlRect = new SDL.SDL_Rect() { x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height };
            SDL.SDL_SetTextInputRect(ref sdlRect);
        }

        public virtual void StartTextComposition()
        {
            if (Enabled)
                return;

            SDL.SDL_StartTextInput();
            Enabled = true;
        }

        public virtual void StopTextComposition()
        {
            if (!Enabled)
                return;

            SDL.SDL_StopTextInput();
            Enabled = false;
        }

        #endregion Public Methods
    }
}