using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shimakaze.Framework
{
    public static class Input
    {
        public static MouseState mouseState;
        public static MouseState lastMouseState;

        public static KeyboardState keyboardState;
        public static KeyboardState lastKeyboardState;

        public static bool IsActive { get; private set; }

        public static void Update(bool isActive)
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();
            UpdateScrollWheel();

            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            IsActive = isActive;
        }


        #region Key Presses

        public static bool KeyWentDown(Keys key)
        {
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public static bool KeyWentUp(Keys key)
        {
            return lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }

        #endregion

        #region Keyboard State

        public static bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        public static bool Shift => keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);

        public static bool Control => keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);

        public static bool Alt => keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);

        #endregion

        #region Mouse State

        public static bool LeftMouseWentDown => IsActive && lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;

        public static bool LeftMouseWentUp => IsActive && lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released;

        public static bool LeftMouseIsDown => IsActive && mouseState.LeftButton == ButtonState.Pressed;

        public static bool LeftMouseIsUp => !IsActive || mouseState.LeftButton == ButtonState.Released;


        public static bool RightMouseWentDown => IsActive && lastMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed;

        public static bool RightMouseWentUp => IsActive && lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released;

        public static bool RightMouseIsDown => IsActive && mouseState.RightButton == ButtonState.Pressed;

        public static bool RightMouseIsUp => !IsActive || mouseState.RightButton == ButtonState.Released;


        public static bool MiddleMouseWentDown => IsActive && lastMouseState.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed;

        public static bool MiddleMouseWentUp => IsActive && lastMouseState.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released;

        public static bool MiddleMouseIsDown => IsActive && mouseState.MiddleButton == ButtonState.Pressed;

        public static bool MiddleMouseIsUp => !IsActive || mouseState.MiddleButton == ButtonState.Released;


        public static Vector2 MousePosition => new Vector2(mouseState.X, mouseState.Y);

        public static Vector2 LastMousePosition => new Vector2(lastMouseState.X, lastMouseState.Y);


        public static Vector2 LeftMouseDrag
        {
            get
            {
                if (IsActive && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed)
                    return new Vector2(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
                else
                    return new Vector2();
            }
        }

        public static Vector2 MiddleMouseDrag
        {
            get
            {
                if (IsActive && mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Pressed)
                    return new Vector2(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
                else
                    return new Vector2();
            }
        }

        public static Vector2 RightMouseDrag
        {
            get
            {
                if (IsActive && mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed)
                    return new Vector2(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
                else
                    return new Vector2();
            }
        }

        #endregion

        #region Mouse Scroll

        public static int scrollWheelAccumulate;
        public static bool ScrollWheelClickUp { get; private set; }
        public static bool ScrollWheelClickDown { get; private set; }
        public static int ScrollWheelClick { get; private set; }

        private static void UpdateScrollWheel()
        {
            ScrollWheelClickUp = ScrollWheelClickDown = false;
            ScrollWheelClick = 0;

            scrollWheelAccumulate += mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;

            if (scrollWheelAccumulate >= 120)
            {
                ScrollWheelClickUp = true;
                ScrollWheelClick = 1;
                scrollWheelAccumulate -= 120;
            }
            if (scrollWheelAccumulate <= -120)
            {
                ScrollWheelClickDown = true;
                ScrollWheelClick = -1;
                scrollWheelAccumulate += 120;
            }
        }

        #endregion

        #region Keyboard Movement

        public static Vector2 SimpleKeyboardMovement
        {
            get
            {
                var move = Vector2.Zero;
                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                    move.X -= 1;
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                    move.X += 1;
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                    move.Y -= 1;
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                    move.Y += 1;
                return move;
            }
        }

        #endregion

    }
}
