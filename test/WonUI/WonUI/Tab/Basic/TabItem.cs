using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.WonUI.Basic
{
    public class TabItem : View
    {
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

        private Button button;
        private TextLabel label;

        public TabItem(string text, string content)
        {
            // Initialize with default values
            BackgroundColor = new Color(0.1f, 0.4f, 0.8f, 1.0f);
            TextColor = Color.White;
            SelectedBackgroundColor = new Color(0.3f, 0.7f, 1.0f, 1.0f);
            SelectedTextColor = Color.White;
            CornerRadius = 20.0f;
            Margin = new Extents(10, 100, 10, 10);
            Padding = new Extents(10, 10, 10, 10);
            Content = content;


            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };


            // Create the tab view
            button = new Button();
            button.Text = text;
            ApplyButtonStyles(button);
            button.Clicked += (sender, e) => OnTabSelected();

            label = new TextLabel();
            label.Text = text;
            ApplyLabelStyles(label);
            label.TouchEvent += (sender, e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Up)
                    OnTabSelected();
                return true;
            };

            Add(button);
            Add(label);

        }

        public void UpdateColors(bool selected)
        {
            button.BackgroundColor = selected ? SelectedBackgroundColor : BackgroundColor;
            button.TextColor = selected ? SelectedTextColor : TextColor;
            label.BackgroundColor = selected ? SelectedBackgroundColor : BackgroundColor;
            label.TextColor = selected ? SelectedTextColor : TextColor;
        }


        private void ApplyButtonStyles(Button button)
        {
            button.BackgroundColor = BackgroundColor;
            button.TextColor = TextColor;
            button.CornerRadius = CornerRadius;
            //button.Margin = Margin;
            //button.Padding = Padding;
        }

        private void ApplyLabelStyles(TextLabel label)
        {
            label.BackgroundColor = BackgroundColor;
            label.TextColor = TextColor;
            //label.Margin = Margin;
            //label.Padding = Padding;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
        }

        protected virtual void OnTabSelected()
        {
            TabSelected?.Invoke(this, Content);
        }
    }
}
