using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;

namespace Tizen.WonUI.Basic
{
   public class TabBar : View
    {
        private List<TabItem> _tabs;
        private int _spacing;

        // Design Properties
        public int Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                if (Layout is LinearLayout linearLayout)
                    linearLayout.CellPadding = new Size2D(value, value);
            }
        }

        public Color TabBarBackgroundColor
        {
            get => BackgroundColor;
            set => BackgroundColor = value;
        }

        // Event
        public delegate void TabSelectedEventHandler(string content);
        public event TabSelectedEventHandler TabSelected;

        public TabBar()
        {
            _tabs = new List<TabItem>();

            // Initialize layout
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Default styles
            WidthResizePolicy = ResizePolicyType.FillToParent;
            HeightResizePolicy = ResizePolicyType.Fixed;
            SizeHeight = 100;
            TabBarBackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f);
            Spacing = 10; // Default spacing
        }

        public void AddTab(TabItem tab)
        {
            _tabs.Add(tab);
            Add(tab);

            tab.TabSelected += (sender, content) =>
            {
                UpdateTabSelection(tab);
                TabSelected?.Invoke(content);
            };
        }

        private void UpdateTabSelection(TabItem selectedTab)
        {
            foreach (var tab in _tabs)
            {
                tab.UpdateColors(tab == selectedTab);                
            }
        }
    }
}
