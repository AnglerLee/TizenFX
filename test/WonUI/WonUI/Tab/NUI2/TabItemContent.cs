using System;
using System.Collections;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.WonUI
{
    public class TabItemContent : View, ITabItemContent
    {
        private TextLabel _label;
        private ImageView _icon;

        public float Spacing { get; set; }

        public TabItemContent()
        {
            Padding = 8;
            Spacing = 8;

            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical
            };
        }

        public string Text
        {
            get => TextLabel.Text;
            set
            {
                TextLabel.Text = value;
            }
        }

        public Color TextColor
        {
            get => TextLabel.TextColor;
            set => TextLabel.TextColor = value;
        }

        public float FontSize
        {
            get => TextLabel.PointSize;
            set => TextLabel.PointSize = value;
        }

        public string FontFamily
        {
            get => TextLabel.FontFamily;
            set => TextLabel.FontFamily = value;
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => TextLabel.HorizontalAlignment;
            set => TextLabel.HorizontalAlignment = value;
        }

        public VerticalAlignment VerticalAlignment
        {
            get => TextLabel.VerticalAlignment;
            set => TextLabel.VerticalAlignment = value;
        }

        public bool Ellipsis
        {
            get => TextLabel.Ellipsis;
            set => TextLabel.Ellipsis = value;
        }

        public bool MultiLine
        {
            get => TextLabel.MultiLine;
            set => TextLabel.MultiLine = value;
        }

        public string ResourceUrl
        {
            get => Icon.ResourceUrl;
            set
            {
                Icon.ResourceUrl = value;
            }
        }


        public bool CropToMask
        {
            get => Icon.CropToMask;
            set => Icon.CropToMask = value;
        }

        public Rectangle Border
        {
            get => Icon.Border;
            set => Icon.Border = value;
        }

        protected TextLabel TextLabel
        {
            get
            {
                if (_label == null)
                {
                    _label = CreateLabel();
                    Add(_label);
                }
                return _label;
            }
        }

        protected ImageView Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = CreateIcon();
                    Add(_icon);
                    //Insert(0, _icon);
                }
                return _icon;
            }
        }

        protected virtual TextLabel CreateLabel()
        {
            return new TextLabel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        protected virtual ImageView CreateIcon()
        {
            return new ImageView
            {
                Size = new Size(24, 24),
                CornerRadius = 0.5f
            };
        }
    }
}
