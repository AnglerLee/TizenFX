using Tizen.NUI2.Layouts;

namespace Tizen.NUI2.Components.Reference
{
    /// <summary>
    /// Default TabItem content.
    /// Label and image icon will be stacked in vertical layout.
    /// </summary>
    public class TabItemContent : VStack, ITabItemContent
    {
        Label _label;
        ImageView _icon;

        /// <summary>
        /// TabItemContent contstructor.
        /// </summary>
        public TabItemContent()
        {
            Padding = (float)8.Dp();
            Spacing = 8.Dp();
        }

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text
        {
            get => TextLabel.Text;
            set
            {
                TextLabel.Text = value;
                TextLabel.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color TextColor
        {
            get => TextLabel.TextColor;
            set
            {
                TextLabel.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size of the text.
        /// </summary>
        public float FontSize
        {
            get => TextLabel.FontSize;
            set
            {
                TextLabel.FontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the font family of the text.
        /// </summary>
        public string FontFamily
        {
            get => TextLabel.FontFamily;
            set
            {
                TextLabel.FontFamily = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public TextAlignment HorizontalAlignment
        {
            get => TextLabel.HorizontalAlignment;
            set
            {
                TextLabel.HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        public TextAlignment VerticalAlignment
        {
            get => TextLabel.VerticalAlignment;
            set
            {
                TextLabel.VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the text should be ellipsized when it overflows its layout bounds.
        /// </summary>
        public bool Ellipsis
        {
            get => TextLabel.Ellipsis;
            set
            {
                TextLabel.Ellipsis = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the text should be multi-line.
        /// </summary>
        public bool MultiLine
        {
            get => TextLabel.MultiLine;
            set
            {
                TextLabel.MultiLine = value;
            }
        }

        /// <summary>
        /// Gets or sets image resource url.
        /// </summary>
        public string ResourceUrl
        {
            get => Icon.ResourceUrl;
            set
            {
                Icon.ResourceUrl = value;
                Icon.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Gets or sets whether the image is a nine-patch image.
        /// </summary>
        public bool IsNinePatchImage
        {
            get => Icon.IsNinePatchImage;
            set
            {
                Icon.IsNinePatchImage = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the image should be cropped to its mask.
        /// </summary>
        public bool CropToMask
        {
            get => Icon.CropToMask;
            set
            {
                Icon.CropToMask = value;
            }
        }

        /// <summary>
        /// Gets or sets the fitting mode used to scale the image to fit the control.
        /// </summary>
        public FittingMode FittingMode
        {
            get => Icon.FittingMode;
            set
            {
                Icon.FittingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the border of the image.
        /// </summary>
        public Thickness Border
        {
            get => Icon.Border;
            set
            {
                Icon.Border = value;
            }
        }

        /// <summary>
        /// Gets the text label.
        /// </summary>
        protected Label TextLabel
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

        /// <summary>
        /// Gets the image icon.
        /// </summary>
        protected ImageView Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = CreateIcon();
                    Insert(0, _icon);
                }

                return _icon;
            }
        }

        public View Body => this;

        public void OnAttachedTo(Component component)
        {
        }

        /// <summary>
        /// Create the default label.
        /// </summary>
        protected virtual Label CreateLabel() => new ()
        {
            HorizontalAlignment = TextAlignment.Center,
            VerticalAlignment = TextAlignment.Center
        };


        /// <summary>
        /// Create the iamge icon.
        /// </summary>
        protected virtual ImageView CreateIcon() => new ()
        {
            DesiredWidth = 24.Dp(),
            DesiredHeight = 24.Dp(),
            CornerRadius = 0.5f
        };
    }
}
