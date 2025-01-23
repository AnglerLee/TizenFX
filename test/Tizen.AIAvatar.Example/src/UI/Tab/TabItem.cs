using System;

namespace Tizen.NUI2.Components.Reference
{
    /// <summary>
    /// TabItem is a selectable component which allows to add tab items for a Tab Container.
    /// One TabItem represents a single page of content using a text and(or) an icon.
    /// </summary>
    public class TabItem : GroupSelectableComponent, ITextElement
    {
        /// <summary>
        /// TabItem constructor.
        /// </summary>
        public TabItem()
        {
        }

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text
        {
            get => TabItemContent.Text;
            set
            {
                TabItemContent.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets image resource url.
        /// </summary>
        public string ResourceUrl
        {
            get => TabItemContent.ResourceUrl;
            set
            {
                TabItemContent.ResourceUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color TextColor
        {
            get => TabItemContent.TextColor;
            set
            {
                TabItemContent.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size of the text.
        /// </summary>
        public float FontSize
        {
            get => TabItemContent.FontSize;
            set
            {
                TabItemContent.FontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the font family of the text.
        /// </summary>
        public string FontFamily
        {
            get => TabItemContent.FontFamily;
            set
            {
                TabItemContent.FontFamily = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public TextAlignment TextHorizontalAlignment
        {
            get => TabItemContent.HorizontalAlignment;
            set
            {
                TabItemContent.HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        public TextAlignment TextVerticalAlignment
        {
            get => TabItemContent.VerticalAlignment;
            set
            {
                TabItemContent.VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the text should be ellipsized when it overflows its layout bounds.
        /// </summary>
        public bool Ellipsis
        {
            get => TabItemContent.Ellipsis;
            set
            {
                TabItemContent.Ellipsis = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the text should be multi-line.
        /// </summary>
        public bool MultiLine
        {
            get => TabItemContent.MultiLine;
            set
            {
                TabItemContent.MultiLine = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the image is a nine-patch image.
        /// </summary>
        public bool IsNinePatchImage
        {
            get => TabItemContent.IsNinePatchImage;
            set
            {
                TabItemContent.IsNinePatchImage = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the image should be cropped to its mask.
        /// </summary>
        public bool IconCropToMask
        {
            get => TabItemContent.CropToMask;
            set
            {
                TabItemContent.CropToMask = value;
            }
        }

        /// <summary>
        /// Gets or sets the fitting mode used to scale the image to fit the control.
        /// </summary>
        public FittingMode IconFittingMode
        {
            get => TabItemContent.FittingMode;
            set
            {
                TabItemContent.FittingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the border of the image.
        /// </summary>
        public Thickness IconBorder
        {
            get => TabItemContent.Border;
            set
            {
                TabItemContent.Border = value;
            }
        }

        /// <summary>
        /// Gets or sets the space between image and text.
        /// </summary>
        public float Spacing
        {
            get => TabItemContent.Spacing;
            set
            {
                TabItemContent.Spacing = value;
            }
        }

        /// <inheritdoc/>
        protected override ContentTemplate GetDefaultContentTemplate() => new (() => new TabItemContent());

        /// <inheritdoc/>
        protected override IConditionalStyle GetDefaultStyle() => GetStyle<TabItem>();

        TextAlignment ITextAlignment.HorizontalAlignment
        {
            get => TextHorizontalAlignment;
            set => TextHorizontalAlignment = value;
        }

        TextAlignment ITextAlignment.VerticalAlignment
        {
            get => TextVerticalAlignment;
            set => TextVerticalAlignment = value;
        }

        ITabItemContent TabItemContent => Content as ITabItemContent;
    }
}
