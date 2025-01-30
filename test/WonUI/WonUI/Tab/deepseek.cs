using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI;

namespace Tizen.WonUI.Deep
{
    public class TabItem<T> where T : View, new()
    {
        public T TabView { get; private set; }
        public string Content { get; set; }

        // Design Properties
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color SelectedBackgroundColor { get; set; }
        public Color SelectedTextColor { get; set; }
        public float CornerRadius { get; set; }
        public Extents Margin { get; set; }
        public Extents Padding { get; set; }

        // Events
        public delegate void TabSelectedEventHandler(object sender, string content);
        public event TabSelectedEventHandler TabSelected;

        public TabItem(string text, string content)
        {
            // Initialize with default values
            BackgroundColor = new Color(0.1f, 0.4f, 0.8f, 1.0f);
            TextColor = Color.White;
            SelectedBackgroundColor = new Color(0.3f, 0.7f, 1.0f, 1.0f);
            SelectedTextColor = Color.White;
            CornerRadius = 20.0f;
            Margin = new Extents(10, 10, 10, 10);
            Padding = new Extents(10, 10, 10, 10);
            Content = content;

            // Create the tab view
            TabView = new T();

            // Apply styles based on view type
            if (TabView is Button button)
            {
                button.Text = text;
                ApplyButtonStyles(button);
                button.Clicked += (sender, e) => OnTabSelected();
            }
            else if (TabView is TextLabel label)
            {
                label.Text = text;
                ApplyLabelStyles(label);
                label.TouchEvent += (sender, e) =>
                {
                    if (e.Touch.GetState(0) == PointStateType.Up)
                        OnTabSelected();
                    return true;
                };
            }
        }

        private void ApplyButtonStyles(Button button)
        {
            button.BackgroundColor = BackgroundColor;
            button.TextColor = TextColor;
            button.CornerRadius = CornerRadius;
            button.Margin = Margin;
            button.Padding = Padding;
        }

        private void ApplyLabelStyles(TextLabel label)
        {
            label.BackgroundColor = BackgroundColor;
            label.TextColor = TextColor;
            label.Margin = Margin;
            label.Padding = Padding;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
        }

        protected virtual void OnTabSelected()
        {
            TabSelected?.Invoke(this, Content);
        }
    }

    public class TabBar<T> : View where T : View, new()
    {
        private List<TabItem<T>> _tabs;
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
            _tabs = new List<TabItem<T>>();

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

        public void AddTab(TabItem<T> tab)
        {
            _tabs.Add(tab);
            Add(tab.TabView);

            tab.TabSelected += (sender, content) =>
            {
                UpdateTabSelection(tab);
                TabSelected?.Invoke(content);
            };
        }

        private void UpdateTabSelection(TabItem<T> selectedTab)
        {
            foreach (var tab in _tabs)
            {
                if (tab.TabView is Button button)
                {
                    button.BackgroundColor = (tab == selectedTab)
                        ? tab.SelectedBackgroundColor
                        : tab.BackgroundColor;
                    button.TextColor = (tab == selectedTab)
                        ? tab.SelectedTextColor
                        : tab.TextColor;
                }
                else if (tab.TabView is TextLabel label)
                {
                    label.BackgroundColor = (tab == selectedTab)
                        ? tab.SelectedBackgroundColor
                        : tab.BackgroundColor;
                    label.TextColor = (tab == selectedTab)
                        ? tab.SelectedTextColor
                        : tab.TextColor;
                }
            }
        }
    }

    public class CustomTabBarExample : NUIApplication
    {
        private TextLabel _contentLabel;

        protected override void OnCreate()
        {
            base.OnCreate();

            Window window = Window.Instance;
            window.BackgroundColor = Color.White;

            // Create content view
            _contentLabel = new TextLabel
            {
                Text = "Home Content",
                TextColor = Color.Black,
                ParentOrigin = ParentOrigin.Center
            };

            View contentView = new View
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                Position = new Position(0, 100),
                BackgroundColor = Color.White
            };
            contentView.Add(_contentLabel);

            // Create custom TabBar with design APIs
            var tabBar = new TabBar<Button>
            {
                TabBarBackgroundColor = Color.LightGray,
                Spacing = 20,
                Padding = new Extents(10, 10, 10, 10)
            };

            // Add customized tabs
            var homeTab = new TabItem<Button>("Home", "Home Content")
            {
                BackgroundColor = Color.Blue,
                SelectedBackgroundColor = Color.Green,
                TextColor = Color.White,
                SelectedTextColor = Color.Yellow,
                Margin = new Extents(5),
                CornerRadius = 10
            };

            var searchTab = new TabItem<Button>("Search", "Search Content")
            {
                BackgroundColor = Color.Red,
                SelectedBackgroundColor = Color.Orange,
                TextColor = Color.White,
                Padding = new Extents(15, 15, 15, 15)
            };

            tabBar.AddTab(homeTab);
            tabBar.AddTab(searchTab);

            // Handle tab selection
            tabBar.TabSelected += (content) => _contentLabel.Text = content;

            window.Add(tabBar);
            window.Add(contentView);
        }

        static void Main(string[] args)
        {
            CustomTabBarExample example = new CustomTabBarExample();
            example.Run(args);
        }
    }
}
namespace Tizen.WonUI.Deep2
{
    public class NuiTabItem : View
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public TextLabel TabText { get; private set; }
        public ImageView TabIcon { get; private set; }
        public View Badge { get; private set; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => UpdateActiveState(value);
        }

        public NuiTabItem() => InitializeComponents();

        private void InitializeComponents()
        {
            // Material Design 기본 스타일 적용
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TabIcon = new ImageView
            {
                Size = new Size(24, 24),
                Margin = new Extents(0, 0, 8, 0)
            };

            TabText = new TextLabel
            {
                TextColor = Color.Black,
                PointSize = 12,
                FontFamily = "Sans"
            };

            Badge = new View
            {
                Size = new Size(16, 16),
                CornerRadius = 8,
                BackgroundColor = Color.Red,
                Hide() // 초기 상태 숨김
            };

            Add(TabIcon);
            Add(TabText);
            Add(Badge);
        }

        private void UpdateActiveState(bool active)
        {
            _isActive = active;
            TabText.TextColor = active ? new Color(0.07f, 0.38f, 0.98f, 1) : Color.Black;
            TabIcon.Opacity = active ? 1.0f : 0.6f;
        }
    }

    public class NuiTabBar : View
    {
        private readonly List<NuiTabItem> _items = new List<NuiTabItem>();
        private View _indicator;

        public event EventHandler<ItemSelectedEventArgs> ItemSelected;

        public NuiTabBar()
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
                ParentOrigin = ParentOrigin.BottomCenter,
                PivotPoint = PivotPoint.BottomCenter
            };

            Add(_indicator);
        }

        public void AddItem(NuiTabItem item)
        {
            item.TouchEvent += OnTabTouched;
            _items.Add(item);
            Add(item);
            UpdateIndicatorPosition();
        }

        private void OnTabTouched(object sender, View.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                var selectedItem = (NuiTabItem)sender;
                SelectItem(selectedItem.Id);
            }
        }

        public void SelectItem(string id)
        {
            foreach (var item in _items)
            {
                item.IsActive = item.Id == id;
            }

            var target = _items.First(i => i.Id == id);
            AnimateIndicator(target);
            ItemSelected?.Invoke(this, new ItemSelectedEventArgs(target));
        }

        private void AnimateIndicator(NuiTabItem target)
        {
            Animation animation = new Animation(300);
            animation.AnimateTo(_indicator, "SizeWidth", target.Size.Width);
            animation.AnimateTo(_indicator, "PositionX", target.PositionX + (target.Size.Width / 2));
            animation.Play();
        }
    }

    public class NuiTabItemBuilder
    {
        private readonly NuiTabItem _item = new NuiTabItem();

        public NuiTabItemBuilder SetText(string text)
        {
            _item.TabText.Text = text;
            return this;
        }

        public NuiTabItemBuilder SetIcon(string resourcePath)
        {
            _item.TabIcon.ResourceUrl = resourcePath;
            return this;
        }

        public NuiTabItemBuilder SetBadge(int count)
        {
            if (count > 0)
            {
                _item.Badge.Show();
                // 배지에 텍스트 추가
                var badgeText = new TextLabel
                {
                    Text = count.ToString(),
                    TextColor = Color.White,
                    PointSize = 8,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _item.Badge.Add(badgeText);
            }
            return this;
        }

        public NuiTabItem Build() => _item;
    }

    public static class NuiTabStyle
    {
        public static void ApplyDefaultTheme(NuiTabBar tabBar)
        {
            tabBar.BackgroundColor = Color.White;
            tabBar.HeightResizePolicy = ResizePolicyType.FillToParent;
            tabBar.WidthResizePolicy = ResizePolicyType.FillToParent;
            tabBar.BoxShadow = new Shadow
            {
                Color = new Color(0, 0, 0, 0.1f),
                BlurRadius = 8,
                Offset = new Vector2(0, 2)
            };
        }
    }

    public class TabBarExample : NUIApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            var window = Window.Instance;
            var mainView = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical
                }
            };

            // 탭 바 생성
            var tabBar = new NuiTabBar();
            NuiTabStyle.ApplyDefaultTheme(tabBar);

            var homeTab = new NuiTabItemBuilder()
                .SetIcon("/res/images/home_icon.png")
                .SetText("Home")
                .Build();

            var settingsTab = new NuiTabItemBuilder()
                .SetIcon("/res/images/settings_icon.png")
                .SetText("Settings")
                .SetBadge(3)
                .Build();

            tabBar.AddItem(homeTab);
            tabBar.AddItem(settingsTab);

            // 컨텐츠 영역
            var contentView = new View
            {
                BackgroundColor = Color.White,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical
                }
            };

            mainView.Add(tabBar);
            mainView.Add(contentView);
            window.Add(mainView);

            tabBar.ItemSelected += (s, e) =>
            {
                contentView.RemoveAllChildren();
                // 선택된 탭에 따른 컨텐츠 업데이트
                var newContent = new TextLabel
                {
                    Text = $"Selected: {e.SelectedItem.TabText.Text}",
                    TextColor = Color.Black
                };
                contentView.Add(newContent);
            };
        }
    }
}