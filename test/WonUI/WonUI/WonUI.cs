using System;
using System.Reflection.Metadata;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.WonUI;
using Tizen.WonUI.Basic;
using Tizen.WonUI.Normal;

namespace WonUI
{
    class Program : NUIApplication
    {

        private Window window;
        private Navigator navigator;
        private TextLabel content;


        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();


        }

        void Initialize()
        {
            window = NUIApplication.GetDefaultWindow();
            navigator = window.GetDefaultNavigator();
            navigator.BackgroundColor = Color.White;
            navigator.Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            var navigationBar = new NavigationBar()
            {
                BackgroundColor = Color.Black,
                };

            var navigationItem = new NavigationItem("hello", "hello", "res.png") {BackgroundColor = Color.Beige,
            Padding = 10,    
            };
            var numericBadge = navigationItem.Badge as NumericBadge;
            numericBadge.Value = 150;
            numericBadge.IsVisible = true;

            var navigationItem2 = new NavigationItem("hello2", "hello2", "res.png") { BackgroundColor = Color.Beige, 
            Padding = 10,
            };

            navigationBar.Add(navigationItem);
            navigationBar.Add(navigationItem2);

            navigator.Add(navigationBar);

        }
        void InitializeNUI2()
        {
            window = NUIApplication.GetDefaultWindow();
            navigator = window.GetDefaultNavigator();
            navigator.BackgroundColor = Color.White;
            navigator.Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            var tabBar = new Tizen.WonUI.TabBar() { BackgroundColor = Color.Black };

            var tab1 = new Tizen.WonUI.TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab1",
                Text = "Tab1",
                HeightResizePolicy = ResizePolicyType.DimensionDependency
            };

            var tab2 = new Tizen.WonUI.TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab2",
                ResourceUrl = "image.png",
            };

            var tab3 = new Tizen.WonUI.TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab3",
                Text = "Tab3",
                ResourceUrl = "image.png"
            };



            tabBar.Add(tab1);
            tabBar.Add(tab2);
            tabBar.Add(tab3);

            navigator.Add(tabBar);
        }

        void InitializeNormal()
        {
            window = NUIApplication.GetDefaultWindow();
            navigator = window.GetDefaultNavigator();
            navigator.BackgroundColor = Color.White;
            navigator.Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
            };


            // 탭 바 생성
            var tabBar = new Tizen.WonUI.Normal.TabBar();
            //Tizen.WonUI.Normal.TabStyle.ApplyDefaultTheme(tabBar);

            var homeTab = new TabItemBuilder()
                .SetIcon("/res/images/home_icon.png")
                .SetText("Home")
                .Build();

            var settingsTab = new TabItemBuilder()
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
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            navigator.Add(tabBar);
            navigator.Add(contentView);



            tabBar.ItemSelected += (s, e) =>
            {
                for (int i = contentView.Children.Count - 1; i >= 0; i--)
                {
                    contentView.Remove(contentView.Children[i]);
                }
                // 선택된 탭에 따른 컨텐츠 업데이트
                var newContent = new TextLabel
                {
                    Text = $"Selected: {e.SelectedItem.TabText.Text}",
                    TextColor = Color.Black,
                };
                contentView.Add(newContent);
            };


        }
        void InitializeBasic()
        {
            window = NUIApplication.GetDefaultWindow();
            navigator = window.GetDefaultNavigator();

            content = new TextLabel()
            {
                Text = "Content#1",
                BackgroundColor = Color.Red,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var tabBar = new Tizen.WonUI.Basic.TabBar
            {
                TabBarBackgroundColor = Color.LightGray,
                Spacing = 20,
                Padding = new Extents(10, 10, 10, 10)
            };

            var homeTab = new Tizen.WonUI.Basic.TabItem("Home", "Home Content")
            {
                BackgroundColor = Color.Blue,
                SelectedBackgroundColor = Color.Green,
                TextColor = Color.White,
                SelectedTextColor = Color.Yellow,
                Margin = new Extents(5),
                CornerRadius = 10
            };

            var searchTab = new Tizen.WonUI.Basic.TabItem("Search", "Search Content")
            {
                BackgroundColor = Color.Red,
                SelectedBackgroundColor = Color.Orange,
                TextColor = Color.White,
                Padding = new Extents(15, 15, 15, 15)
            };


            tabBar.AddTab(homeTab);
            tabBar.AddTab(searchTab);

            tabBar.TabSelected += (value) => content.Text = value;

            navigator.Add(content);
            navigator.Add(tabBar);


        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
