using Tizen.NUI2.Layouts;
using Tizen.NUI2.Markup;

namespace Components.Reference.TestApp.TC
{
    class TabBarTest : HStack, ITestCase
    {
        int leftTabCnt;
        int rightTabCnt;
        public TabBarTest()
        {
            BackgroundColor = Color.White;
            Spacing = 10;
            ItemAlignment = LayoutAlignment.Center;

            leftTabCnt = rightTabCnt = 0;

            CreateLTRTabBar();
            CreateRTLTabBar();
        }

        void CreateLTRTabBar()
        {
            Add(new VStack()
            {
                BackgroundColor = Color.Beige,
                Spacing = 20,
                Children =
                {
                    new Label()
                    {
                        Text = "LTR",
                        HorizontalAlignment = TextAlignment.Center,
                        VerticalAlignment = TextAlignment.Center,
                        DesiredHeight = 100f.Dp()
                    },
                    new TabBar()
                    {
                        BackgroundColor = Color.PowderBlue,
                        Name = "TabBar",
                        Items = {
                            new TabItem()
                            {
                                Name = "Item" + leftTabCnt,
                                Text = "TabItem" + leftTabCnt++,
                                ResourceUrl = "image.png",
                            },
                            new TabItem()
                            {
                                Name = "Item" + leftTabCnt,
                                Text = "TabItem" + leftTabCnt++,
                                ResourceUrl = "image.png",
                            },
                            new TabItem()
                            {
                                Name = "Item" + leftTabCnt,
                                Text = "TabItem" + leftTabCnt++,
                                ResourceUrl = "image.png",
                                IsSelected = true
                            }
                        }
                    }.Self(out var leftTabBar)
                    .HorizontalLayoutAlignment(LayoutAlignment.Center),
                    new VStack()
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Add Tab",
                                ClickedCommand = (_, _) => {
                                    leftTabBar.Add(new TabItem()
                                    {
                                        Name = "Item" + leftTabCnt,
                                        Text = "TabItem" + leftTabCnt++,
                                        ResourceUrl = "image.png",
                                    });
                                },
                            },
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Remove Tab",
                                ClickedCommand = (_, _) => {
                                    if (leftTabCnt <= 0) return;
                                    leftTabCnt--;
                                    leftTabBar.RemoveAt(leftTabCnt);
                                },
                            },
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Clear Tab",
                                ClickedCommand = (_, _) => {
                                    leftTabBar.Clear();
                                    leftTabCnt = 0;
                                },
                            },
                            new Button()
                            {
                                DesiredWidth = 200f.Dp(),
                                Text = "Increase Indicator",
                                ClickedCommand = (_, _) => {
                                    leftTabBar.IndicatorHeight++;
                                },
                            }
                        }
                    }
                }
            }
            );

            leftTabBar.SelectionChanged += OnItemSelectionChanged;
        }

        void CreateRTLTabBar()
        {
            Add(new VStack()
            {
                BackgroundColor = Color.Beige,
                Spacing = 20,
                Children =
                {
                    new Label()
                    {
                        Text = "RTL",
                        HorizontalAlignment = TextAlignment.Center,
                        VerticalAlignment = TextAlignment.Center,
                        DesiredHeight = 100f.Dp()
                    },
                    new TabBar()
                    {
                        BackgroundColor = Color.PowderBlue,
                        Name = "RTLTabBar",
                        LayoutDirection = LayoutDirection.RightToLeft,
                        Items = {
                            new TabItem()
                            {
                                Name = "Item" + rightTabCnt,
                                Text = "TabItem" + rightTabCnt++,
                                ResourceUrl = "image.png",
                            },
                            new TabItem()
                            {
                                Name = "Item" + rightTabCnt,
                                Text = "TabItem" + rightTabCnt++,
                                ResourceUrl = "image.png",
                            },
                            new TabItem()
                            {
                                Name = "Item" + rightTabCnt,
                                Text = "TabItem" + rightTabCnt++,
                                ResourceUrl = "image.png",
                                IsSelected = true
                            }
                        },
                        SelectionChangedCommand = OnItemSelectionChanged
                    }.Self(out var rightTabBar)
                    .HorizontalLayoutAlignment(LayoutAlignment.Center),
                    new VStack()
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Add Tab",
                                ClickedCommand = (_, _) => {
                                    rightTabBar.Add(new TabItem()
                                    {
                                        Name = "Item" + rightTabCnt,
                                        Text = "TabItem" + rightTabCnt++,
                                        ResourceUrl = "image.png",
                                    });
                                },
                            },
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Remove Tab",
                                ClickedCommand = (_, _) => {
                                    if (rightTabCnt <= 0) return;
                                    rightTabCnt--;
                                    rightTabBar.RemoveAt(rightTabCnt);
                                },
                            },
                            new Button()
                            {
                                DesiredWidth = 150f.Dp(),
                                Text = "Clear Tab",
                                ClickedCommand = (_, _) => {
                                    rightTabBar.Clear();
                                    rightTabCnt = 0;
                                },
                            }
                        }
                    }
                }
            }
            );
        }

        private void OnItemSelectionChanged(object o, GroupSelectionChangedEventArgs e)
        {
            Log.Info("TabBarTest", $" Selection Changed {e.OldSelection?.Name} to {e.NewSelection?.Name}");
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
            Dispose();
        }
    }
}
