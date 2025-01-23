using System;
using System.Collections;
using System.Collections.Generic;
using Tizen.NUI2.Layouts;
using Tizen.NUI2.Markup;

namespace Tizen.NUI2.Components.Reference
{
    public class TabBar : TabBar<TabItem>
    {
    }

    /// <summary>
    /// TabBar provides a horizontal layout to display TabItems.
    /// </summary>
    public class TabBar<T> : ContentView, ICollection<T> where T : View, IGroupSelectable
    {
        AbsoluteLayout _layout = new ();
        HStack _itemContainer = new ();
        View _indicator = null;
        Size _lastSize;
        IGroupSelectable _oldSelection = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabBar()
        {
            Body = _layout;
            _layout.Add(_itemContainer);

            Relayout += OnRelayout;
            SelectionGroup.Find(this).SelectionChanged += (s ,e) =>
            {
                T item = (T)e.OldSelection;
                if (e.NewSelection == null)
                {
                    if (Count > 0)
                    {
                        _oldSelection = item;
                        (_itemContainer[0] as T).IsSelected = true;
                        return;
                    }
                }

                UpdateIndicator();

                if (_oldSelection != null)
                {
                    e = new GroupSelectionChangedEventArgs(_oldSelection, e.NewSelection);
                    _oldSelection = null;
                }

                if (e.NewSelection != e.OldSelection)
                {
                    SelectionChanged?.Invoke(this, e);
                    SelectionChangedCommand?.Invoke(this, e);
                }
            };
        }

        /// <summary>
        /// The event that is called when the value of <see cref="Selected"/> changes.
        /// </summary>
        public event EventHandler<GroupSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Selected child changed event command. <see cref="SelectionChanged"/>.
        /// </summary>
        public Action<object, GroupSelectionChangedEventArgs> SelectionChangedCommand { get; set; }

        /// <summary>
        /// Gets the current selected index.
        /// </summary>
        public int SelectedIndex => GetCurrentSelectedIndex();

        /// <summary>
        /// Gets the list of child items.
        /// </summary>
        public ICollection<T> Items => this;

        /// <summary>
        /// Gets the number of child items.
        /// </summary>
        public int Count => _itemContainer.Count;

        /// <summary>
        /// Gets a value indicating whether the list of child items is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the height of the indicator.
        /// </summary>
        public float IndicatorHeight
        {
            get =>  GetIndicator().DesiredHeight;
            set
            {
                GetIndicator().DesiredHeight = value;
                UpdateIndicator(false);
            }
        }

        /// <summary>
        /// Adds an item to Tab bar.
        /// </summary>
        /// <param name="item">The child item to be added.</param>
        public void Add(T item)
        {
            Insert(Count, item);
        }

        /// <summary>
        /// Removes the item from Tab bar.
        /// </summary>
        /// <param name="item">The child item to be removed.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item should not be null.");
            }

            bool isRemoved = false;
            SelectionGroup.Find(this).Remove(item);
            isRemoved = _itemContainer.Remove(item);

            UpdateIndicator();

            return isRemoved;
        }

        /// <summary>
        /// Gets the index of the specified child item in the list of child items.
        /// </summary>
        /// <param name="item">The child item to find the index of.</param>
        /// <returns>The index of the child item, or -1 if it is not found.</returns>
        public int IndexOf(T item)
        {
            return _itemContainer.IndexOf(item);
        }

        /// <summary>
        /// Inserts a child item at the specified index in the list of child items.
        /// </summary>
        /// <param name="index">The index to insert the child item at.</param>
        /// <param name="item">The child item to insert.</param>
        public void Insert(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item should not be null.");
            }

            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            SelectionGroup.Find(this).Insert(index, item);
            _itemContainer.Insert(index, item);

            if (Count == 1)
            {
                item.IsSelected = true;
            }

            UpdateIndicator();
        }

        /// <summary>
        /// Removes the child item at the specified index from the list of child items.
        /// </summary>
        /// <param name="index">The index of the child item to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            SelectionGroup.Find(this).RemoveAt(index);
            _itemContainer.RemoveAt(index);

            UpdateIndicator();
        }

        /// <summary>
        /// Removes all child items from the list of child items.
        /// </summary>
        public void Clear()
        {
            SelectionGroup.Find(this).Clear();
            _itemContainer.Clear();
            _layout.Remove(GetIndicator());
        }

        /// <summary>
        /// Checks if the list of child items contains the specified child item.
        /// </summary>
        /// <param name="item">The child item to check for.</param>
        /// <returns>True if the list of child items contains the child item, false otherwise.</returns>
        public bool Contains(T item)
        {
            return _itemContainer.Contains(item);
        }

        /// <summary>
        /// Copies the child items in the list of child items to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The array to copy the child items to.</param>
        /// <param name="arrayIndex">The index in the array to start copying.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _itemContainer.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of child items.
        /// </summary>
        /// <returns>An enumerator for the list of child items.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_itemContainer.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of child items.
        /// </summary>
        /// <returns>An enumerator for the list of child items.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _itemContainer.GetEnumerator();
        }

        /// <summary>
        /// Create the default indicator.
        /// </summary>
        protected virtual View CreateIndicator()
        {
            return new View()
            {
                Name = "Indicator",
                DesiredHeight = 3f.Dp(),
                BackgroundColor = Color.Black,
            };
        }

        View GetIndicator()
        {
            if (null == _indicator)
            {
                _indicator = CreateIndicator().LayoutBounds(0, 1, -1, -1)
                .LayoutFlags(AbsoluteLayoutFlags.PositionProportional);
            }
            return _indicator;
        }

        void OnRelayout(object sender, EventArgs e)
        {
            if (_lastSize != this.Size())
            {
                UIApplication.Current.PostToUI(() => UpdateIndicator());
                _lastSize = this.Size();
            }
        }

        int GetCurrentSelectedIndex()
        {
            if (SelectionGroup.Find(this).Selected is T selectedItem)
            {
                return IndexOf(selectedItem);
            }
            return -1;
        }

        void EnsureIndicator()
        {
            if (!_layout.Contains(GetIndicator()))
            {
                _layout.Add(GetIndicator());
            }
        }

        void UpdateIndicator(bool isSelectionChanged = true)
        {
            if (SelectedIndex == -1 || Count == 0)
            {
                _layout.Remove(GetIndicator());
                return;
            }
            else
            {
                EnsureIndicator();
            }

            if (isSelectionChanged)
            {
                float indicatorPosition;
                if (LayoutDirection == LayoutDirection.LeftToRight)
                {
                    indicatorPosition = _itemContainer[SelectedIndex].X;
                }
                else
                {
                    indicatorPosition = _itemContainer.Width - _itemContainer[SelectedIndex].X - _itemContainer[SelectedIndex].Width;
                }

                var indicator = GetIndicator();
                indicator.Margin(indicatorPosition, 0, 0, 0);
                if (_itemContainer[SelectedIndex].Width != 0)
                {
                    indicator.DesiredWidth = _itemContainer[SelectedIndex].Width;
                }
            }
        }
    }
}
