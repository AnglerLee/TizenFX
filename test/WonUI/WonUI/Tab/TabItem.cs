using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.WonUI
{
    public class TabItem : View
    {
        private TabItemContent Content;
        public TabItem()
        {
            Content = new TabItemContent();
        }

        public TabItemContent TabItemContent => Content as TabItemContent;

        public bool IsSelected = false;

        public string Text
        {
            get => TabItemContent?.Text;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.Text = value;
            }
        }

        public string ResourceUrl
        {
            get => TabItemContent?.ResourceUrl;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.ResourceUrl = value;
            }
        }

        public Color TextColor
        {
            get => TabItemContent?.TextColor;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.TextColor = value;
            }
        }

        public float FontSize
        {
            get => TabItemContent?.FontSize ?? 0;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.FontSize = value;
            }
        }

        public string FontFamily
        {
            get => TabItemContent?.FontFamily;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.FontFamily = value;
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => TabItemContent?.HorizontalAlignment ?? HorizontalAlignment.Center;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.HorizontalAlignment = value;
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get => TabItemContent?.VerticalAlignment ?? VerticalAlignment.Center;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.VerticalAlignment = value;
            }
        }

        public bool Ellipsis
        {
            get => TabItemContent?.Ellipsis ?? false;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.Ellipsis = value;
            }
        }

        public bool MultiLine
        {
            get => TabItemContent?.MultiLine ?? false;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.MultiLine = value;
            }
        }


        public bool CropToMask
        {
            get => TabItemContent?.CropToMask ?? false;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.CropToMask = value;
            }
        }

        public Rectangle Border
        {
            get => TabItemContent?.Border;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.Border = value;
            }
        }

        public float Spacing
        {
            get => TabItemContent?.Spacing ?? 0;
            set
            {
                if (TabItemContent != null)
                    TabItemContent.Spacing = value;
            }
        }
    }
}
