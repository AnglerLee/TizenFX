using System;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.WonUI
{
    // Badge Interface
    public interface IBadge
    {
        bool IsVisible { get; set; }
        void UpdateLayout();
    }

    // Small Badge Implementation
    public class SmallBadge : View, IBadge
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                if (value) this.Show();
                else this.Hide();
            }
        }

        public SmallBadge()
        {
            Size = new Size(8, 8);
            BackgroundColor = new Color(0.93f, 0.32f, 0.32f, 1.0f); // Material Design default badge color
            CornerRadius = 4;
            IsVisible = true;
        }

        public void UpdateLayout()
        {
            // Position the badge at the top-right corner of the parent

            Position = new Position(
                this.GetParent().GetChildAt(0).SizeWidth - Size.Width / 2,
                -Size.Height / 2
            );
        }
    }

    // Numeric Badge Implementation
    public class NumericBadge : View, IBadge
    {
        private readonly TextLabel _label;
        private bool _isVisible;
        private int _value;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                if (value) this.Show();
                else this.Hide();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateBadgeText();
            }
        }

        public NumericBadge()
        {
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Size = new Size(16, 16);
            BackgroundColor = new Color(0.93f, 0.32f, 0.32f, 1.0f);
            CornerRadius = 8;
            IsVisible = false;

            _label = new TextLabel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = Color.White,
                PointSize = 8
            };
            Add(_label);
        }

        private void UpdateBadgeText()
        {
            _label.Text = _value > 99 ? "99+" : _value.ToString();

            // Adjust badge width based on content
            Size = new Size(
                _value > 99 ? 24 : (_value > 9 ? 16 : 16),
                16
            );
        }

        public void UpdateLayout()
        {
            // Position the badge at the top-right corner of the parent

            Position = new Position(
                this.GetParent().GetChildAt(0).SizeWidth - Size.Width / 2,
                -Size.Height / 2
            );
        }
    }


}