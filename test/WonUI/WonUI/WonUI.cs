using System;
using System.Reflection.Metadata;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.WonUI;

namespace WonUI
{
    class Program : NUIApplication
    {

        private Window window;
        private Navigator navigator;
        private View content;


        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        void Initialize()
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

            var tabbar = new Tizen.WonUI.TabBar()
            {
                BackgroundColor = Color.White,
                Name = "TabBar"
            };

            tabbar.Add(new Tizen.WonUI.TabItem()
            {
                Name = "ItemTab",
                Text = "TabItem1",
                ResourceUrl = "image.png",                
            });

            navigator.Add(content);
            navigator.Add(tabbar);
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
