using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.Xna.Framework;

namespace Shimakaze.Framework.Controls
{
    public class StackPanel : PanelBase
    {
        #region Private Fields

        private Orientation orientation;
        private float spacing;

        #endregion Private Fields

        #region Public Constructors

        public StackPanel() : base()
        {
        }

        public StackPanel(IEnumerable<Control> children) : base(children)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public Orientation Orientation
        {
            get => orientation; set
            {
                orientation = value;
                OnCollectionChanged(null, null);
            }
        }

        public float Spacing
        {
            get => spacing; set
            {
                spacing = value;
                OnCollectionChanged(null, null);
            }
        }

        #endregion Public Properties
        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(Spacing):
                    Spacing = StringConverter.ToSingle(value);
                    return true;
                case nameof(Orientation):
                    Orientation = StringConverter.ToEnum<Orientation>(value);
                    return true;
                default:
                    return false;
            }
        }

        #region Protected Methods

        protected override void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
        {
            Vector2 offset = new(0);
            foreach (var control in Children)
            {
                control.Position = offset;
                offset += Orientation switch
                {
                    Orientation.Vertical => new(0, control.Size.Y + Spacing),
                    Orientation.Horizontal => new(control.Size.X + Spacing, 0),
                    _ => throw new System.NotSupportedException(),
                };
            }
        }

        #endregion Protected Methods
    }
}