/*
 * Copyright(c) 2019 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System.ComponentModel;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;

namespace Tizen.NUI.Components
{
    /// <summary>
    /// ScrollbarStyle is a class which saves Scrollbar's style data.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ScrollbarStyle : ControlStyle
    {
        #region Fields
        static readonly IStyleProperty TrackThicknessProperty = new StyleProperty<Scrollbar, float>((v, o) => v.TrackThickness = o);
        static readonly IStyleProperty ThumbThicknessProperty = new StyleProperty<Scrollbar, float>((v, o) => v.ThumbThickness = o);
        static readonly IStyleProperty TrackColorProperty = new StyleProperty<Scrollbar, Color>((v, o) => v.TrackColor = o);
        static readonly IStyleProperty ThumbColorProperty = new StyleProperty<Scrollbar, Color>((v, o) => v.ThumbColor = o);
        static readonly IStyleProperty TrackPaddingProperty = new StyleProperty<Scrollbar, Extents>((v, o) => v.TrackPadding = o);
        static readonly IStyleProperty ThumbVerticalImageUrlProperty = new StyleProperty<Scrollbar, string>((v, o) => v.ThumbVerticalImageUrl = o);
        static readonly IStyleProperty ThumbHorizontalImageUrlProperty = new StyleProperty<Scrollbar, string>((v, o) => v.ThumbHorizontalImageUrl = o);

        #endregion Fields


        #region Constructors

        /// <summary>
        /// Creates a new instance of a ScrollbarStyle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScrollbarStyle() : base()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="style">Create ScrollbarStyle by style customized by user.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScrollbarStyle(ScrollbarStyle style) : base(style)
        {
        }

        /// <summary>
        /// Static constructor to initialize bindable properties when loading.
        /// </summary>
        static ScrollbarStyle()
        {
        }

        #endregion Constructors


        #region Properties

        /// <summary>
        /// The thickness of the track.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float? TrackThickness
        {
            get => (float?)GetValue(TrackThicknessProperty);
            set => SetValue(TrackThicknessProperty, value);
        }

        /// <summary>
        /// The thickness of the thumb.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float? ThumbThickness
        {
            get => (float?)GetValue(ThumbThicknessProperty);
            set => SetValue(ThumbThicknessProperty, value);
        }

        /// <summary>
        /// The color of the track part.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color TrackColor
        {
            get => (Color)GetValue(TrackColorProperty);
            set => SetValue(TrackColorProperty, value);
        }

        /// <summary>
        /// The color of the thumb part.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        /// <summary>
        /// The padding value of the track.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Extents TrackPadding
        {
            get => (Extents)GetValue(TrackPaddingProperty);
            set => SetValue(TrackPaddingProperty, value);
        }

        /// <summary>
        /// The image url of the vertical thumb.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ThumbVerticalImageUrl
        {
            get => (string)GetValue(ThumbVerticalImageUrlProperty);
            set => SetValue(ThumbVerticalImageUrlProperty, value);
        }

        /// <summary>
        /// The image url of the horizontal thumb.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ThumbHorizontalImageUrl
        {
            get => (string)GetValue(ThumbHorizontalImageUrlProperty);
            set => SetValue(ThumbHorizontalImageUrlProperty, value);
        }

        /// <summary>
        /// The ImageViewStyle of Thumb. internal usage only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal ImageViewStyle Thumb
        {
            get; set;
        }

        #endregion Properties
    }
}
