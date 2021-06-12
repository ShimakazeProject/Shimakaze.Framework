using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework.Collections
{
    public class ControlCollection : ICollection<Control>, INotifyCollectionChanged
    {
        #region Private Fields

        private readonly List<Control> _children = new();
        private readonly Control _parent;

        #endregion Private Fields

        #region Internal Constructors

        internal ControlCollection(Control parent)
        {
            _parent = parent;
        }

        #endregion Internal Constructors

        #region Public Events

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        #endregion Public Events

        #region Public Properties

        public int Count => _children.Count;

        bool ICollection<Control>.IsReadOnly => ((ICollection<Control>)_children).IsReadOnly;

        #endregion Public Properties

        #region Public Indexers

        public Control this[int index] => _children[index];

        #endregion Public Indexers

        #region Public Methods

        public void Add(Control item)
        {
            item.Parent = _parent;
            item.Initialize();
            _children.Add(item);
            CollectionChanged?.Invoke(_parent, new(NotifyCollectionChangedAction.Add, item));
        }

        public void AddRange(IEnumerable<Control> items)
        {
            foreach (var item in items)
            {
                item.Parent = _parent;
                item.Initialize();
            }
            _children.AddRange(items);
            CollectionChanged?.Invoke(_parent, new(NotifyCollectionChangedAction.Add, items));
        }

        public void Clear()
        {
            _children.ForEach(i => i.Parent = null);
            _children.Clear();
            CollectionChanged?.Invoke(_parent, new(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(Control item)
        {
            return _children.Contains(item);
        }

        public void CopyTo(Control[] array, int arrayIndex)
        {
            _children.CopyTo(array, arrayIndex);
        }

        public void ForEach(Action<Control> action)
        {
            _children.ForEach(action);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return ((IEnumerable<Control>)_children).GetEnumerator();
        }

        public bool Remove(Control item)
        {
            item.Parent = null;
            CollectionChanged?.Invoke(_parent, new(NotifyCollectionChangedAction.Remove, item));
            return _children.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }

        #endregion Public Methods
    }
}