using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;

namespace Tizen.WonUI
{
    public class TabBar : View, ICollection<TabItem>
    {
        private View _itemContainer;
        private View _indicator;
        private List<TabItem> _items = new List<TabItem>();

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public TabBar()
        {            
            _itemContainer = new View();
            _itemContainer.Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Add(_itemContainer);
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public float IndicatorHeight
        {
            get => _indicator?.Size.Height ?? 0;
            set
            {
                EnsureIndicator();
                _indicator.Size = new Size(_indicator.Size.Width, value);
                UpdateIndicator();
            }
        }

        public void Add(TabItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _items.Add(item);
            _itemContainer.Add(item);

            //item.Selected += OnItemSelected;

            if (_items.Count == 1)
                item.IsSelected = true;

            UpdateIndicator();
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            var selectedItem = sender as TabItem;
            foreach (var item in _items)
            {
                if (item != selectedItem)
                    item.IsSelected = false;
            }

            UpdateIndicator();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selectedItem));
        }

        public bool Remove(TabItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool removed = _items.Remove(item);
            if (removed)
            {
                _itemContainer.Remove(item);
                UpdateIndicator();
            }

            return removed;
        }

        public void Clear()
        {
            _items.Clear();
            //_itemContainer.RemoveAll();
            _indicator?.Dispose();
            _indicator = null;
        }

        public bool Contains(TabItem item) => _items.Contains(item);

        public void CopyTo(TabItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TabItem> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EnsureIndicator()
        {
            if (_indicator == null)
            {
                _indicator = new View
                {
                    Size = new Size(0, 3),
                    BackgroundColor = Color.Black
                };
                Add(_indicator);
            }
        }

        private void UpdateIndicator()
        {
            EnsureIndicator();

            var selectedItem = _items.Find(item => item.IsSelected);
            if (selectedItem == null)
            {
                return;
            }

            _indicator.Position = new Position(
                selectedItem.Position.X,
                _itemContainer.Size.Height - _indicator.Size.Height
            );
            _indicator.Size = new Size(selectedItem.Size.Width, _indicator.Size.Height);
        }
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public TabItem SelectedItem { get; }

        public SelectionChangedEventArgs(TabItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }
}
