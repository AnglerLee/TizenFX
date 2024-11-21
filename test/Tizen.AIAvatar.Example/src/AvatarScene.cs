/*
 * Copyright(c) 2023 Samsung Electronics Co., Ltd.
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

using global::System;
using System.IO;
using Tizen.NUI;
using Tizen.NUI.Scene3D;


namespace AIAvatar
{
    public class AvatarScene : SceneView
    {
        private static string resourcePath = Utils.ResourcePath;

        private const int cameraAnimationDurationMilliSeconds = 2000;
        private const int sceneTransitionDurationMilliSeconds = 1500;

        private AIAvatar defaultAIAvatar;
        
        private bool isBlink = false;
        private bool isShowing = true;

        private float iblFactor = 0.3f;

        public AIAvatar AIAvatar
        {
            get => defaultAIAvatar;
        }

        public float IBLFactor
        {
            get
            {
                return iblFactor;
            }
            set
            {
                iblFactor = value;
                ImageBasedLightScaleFactor = value;
            }
        }
        
        public AvatarScene()
        {
            PivotPoint = Tizen.NUI.PivotPoint.TopLeft;
            ParentOrigin = Tizen.NUI.ParentOrigin.TopLeft;
            PositionUsesPivotPoint = true;

            // Setup Image Based Light
            SetupSceneViewIBL();

            // Setup camera preset
            SetupDefaultCamera();

            // Setup Default Avatar Position & Orientation
            SetupDefaultAvatar();

        }


        public void SetupSceneViewIBLFactor(float value)
        {
            IBLFactor = value;
        }


        public void ShowHide()
        {
            if (!isShowing)
            {
                
            }
            else
            {
                
            }
            isShowing = !isShowing;
        }

        public void InintTTsTest()
        {

        }

        public void StartTTSTest()
        {
           
        }

        public void StopTTSTest()
        {
           
        }

        public void SwitchCamera()
        {
            CameraTransition(1, cameraAnimationDurationMilliSeconds);
        }

        public void SetupSceneViewCameraFov(float value)
        {
            var camera = GetSelectedCamera();
            camera.FieldOfView = new Radian(value);
        }

        public void ChangeAvatar()
        {
            DestroyAvatar();
            CreateAvatar();

        }

        internal static string GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }



        private byte[] ReadAllBytes(string path)
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                return bytes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SetupDefaultAvatar()
        {
            defaultAIAvatar = CreateAvatar();
            Add(defaultAIAvatar);
            
            
            
        }

        private AIAvatar CreateAvatar()
        {
            AIAvatar avatar = new AIAvatar(resourcePath + "/Model/Default/model_external.gltf")
            {
                Position = new Position(0.0f, -1.70f, -2.0f),
                Size = new Size(1.0f, 1.0f, 1.0f),
                Orientation = new Rotation(new Radian(new Degree(0.0f)), Vector3.YAxis)
            };
                        

            avatar.ResourcesLoaded += (sender, args) => 
            {
                avatar.Initialize();
            };

            return avatar;
            
        }

        private void DestroyAvatar()
        {
            defaultAIAvatar.Dispose();
        }

    

        private void SetupSceneViewIBL()
        {
            SetImageBasedLightSource(resourcePath + "Images/" + "Irradiance.ktx", resourcePath + "Images/" + "Radiance.ktx", IBLFactor);
        }

        private void SetupDefaultCamera()
        {
            // Default camera setting
            // Note : SceneView always have 1 default camera.
            var defaultCamera = GetCamera(0u);

            defaultCamera.PositionX = 0.0f;
            defaultCamera.PositionY = -2.3f;
            defaultCamera.PositionZ = 0.0f;
            defaultCamera.FieldOfView = new Radian(new Degree(45.0f));
        }
    }
}
