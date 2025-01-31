using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;

namespace Tizen.WonUI.Normal
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public TabItem SelectedItem { get; private set; }

        public ItemSelectedEventArgs(TabItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

   public class TabBar : View
    {
        private readonly List<TabItem> _items = new List<TabItem>();
        private View _indicator;

        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        public TabBar()
        {
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                CellPadding = new Size2D(32, 0)
            };

            _indicator = new View
            {
                Size = new Size(0, 2),
                BackgroundColor = new Color(0.07f, 0.38f, 0.98f, 1),
                PositionUsesPivotPoint = true,
                ParentOrigin = Tizen.NUI.ParentOrigin.BottomCenter,
                PivotPoint = Tizen.NUI.PivotPoint.BottomCenter
            };

            Add(_indicator);
        }

        private bool OnTabTouched(object sender, View.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                var selectedItem = (TabItem)sender;
                SelectItem(selectedItem);
                return true;
            }
            return false;
        }

        public void AddItem(TabItem item)
        {
            item.TouchEvent += OnTabTouched;
            _items.Add(item);
            Add(item);
            UpdateIndicatorPosition();
        }

    

        public void SelectItem(TabItem selectItem)
        {
            foreach (var item in _items)
            {
                item.IsActive = item == selectItem;
            }
            
            AnimateIndicator(selectItem);
            ItemSelected?.Invoke(this, new ItemSelectedEventArgs(selectItem));
        }

        private void UpdateIndicatorPosition()
        {
            if (_items.Count > 0)
            {
                var firstItem = _items[0];
                _indicator.SizeWidth = firstItem.Size.Width;
                _indicator.PositionX = firstItem.PositionX;
                
            }
        }

        private void AnimateIndicator(TabItem target)
        {
            Animation animation = new Animation(300);
            animation.AnimateTo(_indicator, "SizeWidth", target.Size.Width);
            animation.AnimateTo(_indicator, "PositionX", target.PositionX + (target.Size.Width / 2));
            Log.Debug("UI", $"{target.Size.Width}, {target.PositionX} ");
            animation.Play();
        }
    }

    public static class TabStyle
    {
        public static void ApplyDefaultTheme(TabBar tabBar)
        {
            tabBar.BackgroundColor = Color.White;
            tabBar.HeightResizePolicy = ResizePolicyType.FillToParent;
            tabBar.WidthResizePolicy = ResizePolicyType.FillToParent;
        }
    }
}
