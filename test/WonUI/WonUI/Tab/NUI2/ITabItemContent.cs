using System;
using System.Collections;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.WonUI
{
    public interface ITabItemContent
    {
        float Spacing { get; set; }

        #region Image
            /// <summary>
            /// Gets or sets the resource url of image.
            /// </summary>
            string ResourceUrl
            {
                get;
                set;
            }
            /// <summary>
            /// Gets or sets whether the image should be cropped to its mask.
            /// </summary>
            bool CropToMask
            {
                get;
                set;
            }
        #endregion

        #region Text
            /// <summary>
            /// Gets or sets the text content of the object.
            /// </summary>
            string Text { get; set; }

            /// <summary>
            /// Gets or sets the color of the text.
            /// </summary>
            Color TextColor { get; set; }

            /// <summary>
            /// Gets or sets the font size of the text.
            /// </summary>
            float FontSize { get; set; }

            /// <summary>
            /// Gets or sets the font family of the text.
            /// </summary>
            string FontFamily
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets whether the text should be ellipsized when it overflows its layout bounds.
            /// </summary>
            bool Ellipsis
            {
                get;
                set;
            }


            /// <summary>
            /// Gets or sets whether the text should be multi-line.
            /// </summary>
            bool MultiLine
            {
                get;
                set;
            }

        #endregion
    }

}
