using System;

using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.EventArguments
{
    public class MouseMoveEventArgs : EventArgs
    {
        internal MouseMoveEventArgs(Vector2 mousePosition, Vector2 lastMousePosition)
        {
            MousePosition = mousePosition;
            LastMousePosition = lastMousePosition;
        }

        public Vector2 MousePosition { get; }
        public Vector2 LastMousePosition { get; }
    }
}
