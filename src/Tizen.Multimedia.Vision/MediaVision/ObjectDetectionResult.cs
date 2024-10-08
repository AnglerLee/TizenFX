/*
 * Copyright (c) 2019 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace Tizen.Multimedia.Vision
{
    /// <summary>
    /// Provides the ability to get the result of object detection using <see cref="InferenceModelConfiguration"/> and
    /// <see cref="ObjectDetector"/>.
    /// </summary>
    /// <since_tizen> 6 </since_tizen>
    [Obsolete("Deprecated since API12. Will be removed in API15.")]
    public class ObjectDetectionResult
    {
        internal ObjectDetectionResult(int indice, string name, float confidence,
            global::Interop.MediaVision.Rectangle location)
        {
            Indice = indice;
            Name = name;
            Confidence = confidence;
            Location = location.ToApiStruct();
        }

        /// <summary>
        /// Gets the indice of detected object.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public int Indice { get; }

        /// <summary>
        /// Gets the name of detected object.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public string Name { get; }

        /// <summary>
        /// Gets the confidence of detected object.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public float Confidence { get; }

        /// <summary>
        /// Gets the location of detected object.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public Rectangle Location { get; }
    }
}
