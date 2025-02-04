using System;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.WonUI
{
    // Abstract Base Navigation Bar
    public abstract class NavigationBarBase : View
    {
        protected List<INavigationItem> Items { get; } = new List<INavigationItem>();
        public event EventHandler<INavigationItem> ItemSelected;

        protected NavigationBarBase()
        {
            // Common initialization
        }

        public void Add(INavigationItem item)
        {
            Items.Add(item);
            OnItemAdded(item);
        }

        protected abstract void OnItemAdded(INavigationItem item);
        protected abstract void UpdateLayout();

        protected virtual void OnItemSelected(INavigationItem item)
        {
            foreach (var navItem in Items)
            {
                navItem.IsSelected = navItem.Id == item.Id;
            }

            ItemSelected?.Invoke(this, item);
        }
    }

    // Material Design Navigation Bar Implementation
    public class NavigationBar : NavigationBarBase
    {
        private readonly View _itemContainer;

        public NavigationBar()
        {
            _itemContainer = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal
                }
            };
            Add(_itemContainer);
        }

        protected override void OnItemAdded(INavigationItem item)
        {
            if (item is View viewItem)
            {
                _itemContainer.Add(viewItem);
                viewItem.TouchEvent += OnTabTouched;
                UpdateLayout();
            }
        }

        protected override void UpdateLayout()
        {
            // Implement Material Design specific layout logic
            
        }

        private bool OnTabTouched(object sender, View.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                var selectedItem = (INavigationItem)sender;
                OnItemSelected(selectedItem);
                return true;
            }
            return false;
        }
    }

}