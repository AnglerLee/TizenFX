using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.WonUI.Normal
{
     public class TabItem : View
    {
        public TextLabel TabText { get; private set; }
        public ImageView TabIcon { get; private set; }
        public View Badge { get; private set; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => UpdateActiveState(value);
        }

        public TabItem() => InitializeComponents();

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
}
