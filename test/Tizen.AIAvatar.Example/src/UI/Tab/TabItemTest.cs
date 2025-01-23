using Tizen.NUI2.Layouts;
using Tizen.NUI2.Markup;

namespace Components.Reference.TestApp.TC
{
    class TabItemTest : VStack, ITestCase
    {
        public TabItemTest()
        {
            BackgroundColor = Color.White;
            Spacing = 10;
            ItemAlignment = LayoutAlignment.Center;

            Add(new Label()
            {
                Text = "No Tab clicked"
            }
            .Self(out var label)
            .HorizontalLayoutAlignment(LayoutAlignment.Center));

            Add(new TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab1",
                Text = "Tab1",
                ClickedCommand = (s, _) => label.Text = "Only Text Tab is clicked"
            }
            .HorizontalLayoutAlignment(LayoutAlignment.Center));

            Add(new TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab2",
                ResourceUrl = "image.png",
                ClickedCommand = (s, _) => label.Text = "Only Icon Tab is clicked"
            }
            .HorizontalLayoutAlignment(LayoutAlignment.Center));

            Add(new TabItem()
            {
                BackgroundColor = Color.Beige,
                Name = "Tab3",
                Text = "Tab3",
                ResourceUrl = "image.png",
                ClickedCommand = (s, _) => label.Text = "Tab (Icon+Text) is clicked"
            }
            .HorizontalLayoutAlignment(LayoutAlignment.Center));

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
