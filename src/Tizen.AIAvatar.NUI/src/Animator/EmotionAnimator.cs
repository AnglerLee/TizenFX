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
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Tizen.NUI.Scene3D;

namespace Tizen.AIAvatar.NUI
{ 
    public class EmotionAnimator
    {

        private EmotionConfig EmotionConfigData;
        private Dictionary<string, List<MotionData>> faceAnimationDataByCategory;

        public void LoadEmotionConfig(in string faceMotionResourcePath, in string filePath)
        {
            try
            {
                string json = File.ReadAllText(faceMotionResourcePath + filePath);
                EmotionConfigData = JsonSerializer.Deserialize<EmotionConfig>(json);

                //LoadExpressionData(faceMotionResourcePath);

            }
            catch (JsonException ex)
            {                
                throw new Exception($"Error loading Emotion Config data from {filePath}: {ex}");
            }


            faceAnimationDataByCategory = new Dictionary<string, List<MotionData>>();

            foreach (Expression expression in EmotionConfigData.expressions)
            {
                if (!faceAnimationDataByCategory.ContainsKey(expression.name))
                {
                    faceAnimationDataByCategory[expression.name] = new List<MotionData>();
                }

                foreach (string filename in expression.filename)
                {                    
                    string expressionFile = File.ReadAllText(faceMotionResourcePath + "/" + filename);
                    FaceAnimationData expressionFaceAnimationData = JsonSerializer.Deserialize<FaceAnimationData>(expressionFile);
                    MotionData expressionFaceMotionData = AnimationLoader.Instance.CreateFacialMotionData(expressionFaceAnimationData, EmotionConfigData.ignoreBlendShapes);
                    faceAnimationDataByCategory[expression.name].Add(expressionFaceMotionData);
                }

            }
        }
    }
}
