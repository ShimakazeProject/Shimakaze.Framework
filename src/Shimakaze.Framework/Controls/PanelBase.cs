using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using FontStashSharp;

using Microsoft.Xna.Framework;

using Shimakaze.Framework.Collections;

using static System.Net.Mime.MediaTypeNames;

namespace Shimakaze.Framework.Controls
{
    public abstract class PanelBase : TextureBlock, ICollection<Control>
    {
        #region Protected Constructors

        protected PanelBase()
        {
            Children = new(this);
            Children.CollectionChanged += OnCollectionChanged;
        }
        protected PanelBase(IEnumerable<Control> children) : this() => Children.AddRange(children);

        #endregion Protected Constructors

        #region Public Properties

        public bool AutoSize { get; set; } = true;

        public ControlCollection Children { get; }

        public override Vector2 Size
        {
            get
            {
                if (AutoSize)
                {
                    Vector2 tmp = new(0);
                    for (int i = 0; i < Children.Count; i++)
                        tmp += Children[i].Size;
                    base.Size = tmp;
                }
                return base.Size;
            }
            set
            {
                base.Size = value;
                AutoSize = false;
            }
        }

        int ICollection<Control>.Count => ((ICollection<Control>)Children).Count;

        bool ICollection<Control>.IsReadOnly => ((ICollection<Control>)Children).IsReadOnly;

        #endregion Public Properties
        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(AutoSize):
                    AutoSize = StringConverter.ToBoolean(value);
                    return true;
                default:
                    return false;
            }
        }

        #region Protected Methods

        protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
        {
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            Children.ForEach(i => i.Dispose());
        }

        #endregion Protected Methods

        #region Public Methods

        public void Add(Control item)
        {
            ((ICollection<Control>)Children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Control>)Children).Clear();
        }

        public bool Contains(Control item)
        {
            return ((ICollection<Control>)Children).Contains(item);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Draw(gameTime);
            Children.ForEach(i => i.Draw(gameTime));
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return ((IEnumerable<Control>)Children).GetEnumerator();
        }

        public bool Remove(Control item)
        {
            return ((ICollection<Control>)Children).Remove(item);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);
            Children.ForEach(i => i.Update(gameTime));
        }

        void ICollection<Control>.CopyTo(Control[] array, int arrayIndex)
        {
            ((ICollection<Control>)Children).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }

        #endregion Public Methods
    }
}