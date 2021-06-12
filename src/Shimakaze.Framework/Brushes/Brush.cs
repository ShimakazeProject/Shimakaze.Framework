using System;

using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.Brushes
{
    public abstract class Brush : IDisposable
    {
        #region Private Fields

        private bool disposedValue;

        #endregion Private Fields

        #region Private Destructors

        ~Brush()
        {
            Dispose(disposing: false);
        }

        #endregion Private Destructors

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                disposedValue = true;
        }

        #endregion Protected Methods

        #region Protected Internal Methods

        protected internal abstract void Draw(GraphicsManager graphics, ref Vector2 position, ref Vector2 size);

        #endregion Protected Internal Methods

        #region Public Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods
    }
}