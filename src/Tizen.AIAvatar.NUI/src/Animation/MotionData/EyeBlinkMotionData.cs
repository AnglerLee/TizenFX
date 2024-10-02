/*
 * Copyright(c) 2024 Samsung Electronics Co., Ltd.
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
 */

using System;
using Tizen.NUI;
using Tizen.NUI.Scene3D;
using static Tizen.NUI.AlphaFunction;

namespace Tizen.AIAvatar
{
    internal class EyeBlinkMotionData : ICustomMotionData, IDisposable
    {
        private MotionData motionData;
        private MotionValue eyeBlinkMotionValue;

        private AvatarBlendShapeIndex headBlendShapeEyeLeft;
        private AvatarBlendShapeIndex headBlendShapeEyeRight;
        private AvatarBlendShapeIndex eyelashBlendShapeEyeLeft;
        private AvatarBlendShapeIndex eyelashBlendShapeEyeRight;

        private int duration;
        private const int DefaultBlinkDuration = 4000;

        public void Initialize(AvatarProperties avatarProperties)
        {
            if (avatarProperties == null)
            {
                throw new ArgumentNullException(nameof(avatarProperties), "Avatar properties cannot be null.");
            }

            CreateEyeBlinkData(avatarProperties);
            motionData = GenerateMotionData(DefaultBlinkDuration);
        }

        public MotionData GetMotionData(int durationMilliseconds = DefaultBlinkDuration)
        {
            if (motionData == null)
            {
                throw new InvalidOperationException("Animation is not initialized.");
            }

            if (durationMilliseconds != duration)
            {
                motionData?.Dispose();
                motionData = GenerateMotionData(durationMilliseconds);
            }

            return motionData;
        }

        public void Dispose()
        {
            motionData?.Dispose();
            eyeBlinkMotionValue.Dispose();
            headBlendShapeEyeLeft.Dispose();
            headBlendShapeEyeRight.Dispose();
            eyelashBlendShapeEyeLeft.Dispose();
            eyelashBlendShapeEyeRight.Dispose();
        }

        private MotionData GenerateMotionData(int durationMilliseconds)
        {
            duration = durationMilliseconds;

            var motionData = new MotionData(duration);

            motionData.Add(headBlendShapeEyeLeft, eyeBlinkMotionValue);
            motionData.Add(headBlendShapeEyeRight, eyeBlinkMotionValue);
            motionData.Add(eyelashBlendShapeEyeLeft, eyeBlinkMotionValue);
            motionData.Add(eyelashBlendShapeEyeRight, eyeBlinkMotionValue);

            return motionData;
        }

        private void CreateEyeBlinkData(AvatarProperties avatarProperties)
        {
            using var keyFrames = new KeyFrames();
            using var alphaFunction = new AlphaFunction(BuiltinFunctions.EaseInOut);
            
            keyFrames.Add(0.0f, 0.0f, alphaFunction);
            keyFrames.Add(0.1f, 0.0f, alphaFunction);
            keyFrames.Add(0.15f, 1.0f, alphaFunction);
            keyFrames.Add(0.2f, 0.0f, alphaFunction);
            keyFrames.Add(0.3f, 0.0f, alphaFunction);
            keyFrames.Add(0.325f, 1.0f, alphaFunction);
            keyFrames.Add(0.35f, 0.0f, alphaFunction);
            keyFrames.Add(1.0f, 0.0f, alphaFunction);


            eyeBlinkMotionValue = new MotionValue(keyFrames);

            headBlendShapeEyeLeft = new AvatarBlendShapeIndex(avatarProperties.NodeMapper, NodeType.HeadGeo, avatarProperties.BlendShapeMapper, BlendShapeType.EyeBlinkLeft);
            headBlendShapeEyeRight = new AvatarBlendShapeIndex(avatarProperties.NodeMapper, NodeType.HeadGeo, avatarProperties.BlendShapeMapper, BlendShapeType.EyeBlinkRight);
            eyelashBlendShapeEyeLeft = new AvatarBlendShapeIndex(avatarProperties.NodeMapper, NodeType.EyelashGeo, avatarProperties.BlendShapeMapper, BlendShapeType.EyeBlinkLeft);
            eyelashBlendShapeEyeRight = new AvatarBlendShapeIndex(avatarProperties.NodeMapper, NodeType.EyelashGeo, avatarProperties.BlendShapeMapper, BlendShapeType.EyeBlinkRight);
        }


    }
}
