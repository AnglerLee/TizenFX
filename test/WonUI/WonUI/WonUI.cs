using System;
using System.Reflection.Metadata;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.WonUI.Basic;

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
            InitializeBasic();
        }

        void Initialize()
        {
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

            var tabBar = new  Tizen.WonUI.Basic.TabBar
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
