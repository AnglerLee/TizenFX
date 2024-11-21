using System;
using System.Collections.Generic;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.Scene3D;
using Tizen.AIAvatar;
using Tizen.AIAvatar.NUI;


namespace AIAvatar
{
    public partial class AIAvatar : Model
    {

        private LipSyncer lipSyncer;
        private Audio2Vowels audio2Vowels;

        private WaveData waveData;
        private AudioPlayer audioPlayer;
        private AudioOptions audioOptions;
                        
        private SerialAnimator bodyAnimator = new SerialAnimator();
        private ParallelAnimator faceAnimator = new ParallelAnimator();

        private List<MotionInfo> faceMotions = new List<MotionInfo>();
        private List<MotionInfo> bodyMotions = new List<MotionInfo>();
        private List<MotionInfo> customMotions = new List<MotionInfo>();

        private readonly string BodyMotionResourcePath = "/Model/Animation/Body/";
        private readonly string FaceMotionResourcePath = "/Model/Animation/Face/";

        private string[] testVowels = { "sil", "A", "E", "I", "O", "U", "A", "E", "I", "O", "U", "sil", "ER", "HM", "sil", "sil" };

        


        public AIAvatar() : base()
        {
            LoadResources();
        }
        public AIAvatar(string avatarUrl, string resourceDirectoryUrl = "") : base(avatarUrl, resourceDirectoryUrl)
        {
            LoadResources();
        }
        public AIAvatar(AIAvatar avatar) : base(avatar)
        {
            LoadResources();
        }


        private void LoadResources()
        {
            try
            {
                faceMotions.AddRange(
                    AnimationLoader.Instance.LoadFaceMotions(Utils.ResourcePath + FaceMotionResourcePath)
                    );

                bodyMotions.AddRange(
                    AnimationLoader.Instance.LoadBodyMotions(
                        Utils.ResourcePath + BodyMotionResourcePath,
                        true,
                        new Vector3(0.01f, 0.01f, 0.01f)
                        )
                    );
            }
            catch (Exception e)
            {
                Log.Error(Utils.LogTag, $"LoadResource : {e.Message}");
            }

            LoadAudioResource();
        }

        private void LoadAudioResource()
        {
            waveData = Utils.LoadWave(Utils.ResourcePath + "/Voice/en_tts.wav");
            Log.Info(Utils.LogTag, $"{waveData.NumChannels}, {waveData.SampleRate}");
        }



        public void Initialize()
        {

            InitializeBodyAnimations();
            bodyAnimator.AnimatorStateChanged += OnAnimationStateChanged;

            InitializeFaceAnimations();
            InitializeCustomAnimations();
            faceAnimator.AnimatorStateChanged += OnAnimationStateChanged;
            
            InitialzeLipSync();
            lipSyncer.AnimatorStateChanged += OnAnimationStateChanged;


            InitializeAudioPlayer();       
            
            
        }

        public void PlayRandomBodyAnimation()
        {
            try
            {
                Random random = new Random();
                int index = random.Next(0, bodyAnimator.Count);
                uint key = bodyAnimator.GetKeyElementAt(index);
                bodyAnimator.Play(key);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.LogTag, "An error occurred while Play the Body Animaton: " + ex.Message);
            }
        }

        public void PlayMultipleFacialAnimations()
        {
            try
            {
                uint blinkKey = faceAnimator.GetIndexByName("EyeBlink");
                uint key = faceAnimator.GetKeyElementAt(0);
                faceAnimator.Play(new List<uint> { key, blinkKey });
            }
            catch (Exception ex)
            {
                Log.Error(Utils.LogTag, "An unexpected exception occurred: " + ex.Message);
            }
        }




        public void PlayLipSync()
        {
            try
            {
                Animation lipAnimation = lipSyncer.GenerateAnimationFromVowels(testVowels, 0.08f);
                lipSyncer.Enqueue(lipAnimation);
                lipSyncer.Play();

            }
            catch (Exception ex)
            {
                Log.Error(Utils.LogTag, "An unexpected exception occurred: " + ex.Message);
            }
        }

        public void PlayAudioLipSync()
        {
            audio2Vowels.SetSampleRate(waveData.SampleRate);
            testVowels = audio2Vowels.PredictVowels(waveData.RawAudioData);
            Log.Info(Utils.LogTag, $"{string.Join(", ", testVowels)}");

            audioPlayer.Play(waveData.RawAudioData, waveData.SampleRate);
            PlayLipSync();
        }

        public void PlayAudioLipSyncStream()
        {
            audio2Vowels.SetSampleRate(waveData.SampleRate);
            testVowels = audio2Vowels.PredictVowels(waveData.RawAudioData);
            Log.Info(Utils.LogTag, $"{string.Join(", ", testVowels)}");

            audioPlayer.Play(waveData.RawAudioData, waveData.SampleRate);
            PlayStreamingLipSync();
        }



            public void PauseAnimations()
        {
            bodyAnimator.Pause();
            faceAnimator.Pause();
            lipSyncer.Pause();
        }


        public void StopAnimations()
        {
            bodyAnimator.Stop();
            faceAnimator.Stop();
            lipSyncer.Stop();
        }

        public void StartEyeBlink()
        {
            try
            {
                faceAnimator.Play("EyeBlink");
            }
            catch (Exception ex)
            {
                Log.Error(Utils.LogTag, "An error occurred while Play the eye blink: " + ex.Message);
            }
        }


        private void InitializeFaceAnimations()
        {
            try
            {
                foreach (var motion in faceMotions)
                {
                    var faceAnim = GenerateMotionDataAnimation(motion.MotionData);
                    faceAnimator.Add(faceAnim, motion.MotionName);
                }
            }
            catch (Exception e)
            {
                Log.Error(Utils.LogTag, $"InitializeFaceAnimations : {e.Message}");
            }
        }

        private void InitializeBodyAnimations()
        {
            try
            {
                foreach (var motion in bodyMotions)
                {
                    var bodyAnim = GenerateMotionDataAnimation(motion.MotionData);
                    bodyAnim.BlendPoint = 0.2f;
                    bodyAnimator.Add(bodyAnim, motion.MotionName);
                }
            }
            catch (Exception e)
            {
                Log.Error(Utils.LogTag, $"Initialize : {e.Message}");
            }
        }

        private void InitializeCustomAnimations()
        {
            var eyeBlinkMotionData = new EyeBlinkMotionData();

            var eyeAnimation = GenerateMotionDataAnimation(eyeBlinkMotionData.GetMotionData(4000));

            if (eyeAnimation != null)
            {
                eyeAnimation.LoopCount = 0;
                faceAnimator.Add(eyeAnimation, "EyeBlink");
            }
            else
            {
                Log.Error(Utils.LogTag, "Failed to initialize custom animations for avatar.");
            }
        }

        private void InitialzeLipSync()
        {
            lipSyncer = new LipSyncer();
            lipSyncer.Initialize(this, Utils.ResourcePath + "/Model/emoji_viseme_blendshapes.json");
        }

        private void InitializeAudioPlayer()
        {   
            audioOptions = new AudioOptions(24000, Tizen.Multimedia.AudioChannel.Mono, Tizen.Multimedia.AudioSampleType.S16Le, Tizen.Multimedia.AudioStreamType.System);
            audioOptions.DuckingOptions(Tizen.Multimedia.AudioStreamType.Media, 500, 0.2);

            audioPlayer = new AudioPlayer(audioOptions);

            audio2Vowels = new Audio2Vowels(Utils.ResourcePath + "/Intelligence/LipSync/audio2vowel_7.tflite");


        }

        private void OnAnimationStateChanged(object sender, AnimatorChangedEventArgs e)
        {

            switch (e.Current)
            {
                case AnimatorState.Ready:
                    Tizen.Log.Info(Utils.LogTag, $"[{e.Message}]  {sender.ToString()} State is Ready");
                    break;

                case AnimatorState.Playing:
                    Tizen.Log.Info(Utils.LogTag, $"[{e.Message}]  {sender.ToString()} State is Playing");
                    break;

                case AnimatorState.Paused:
                    Tizen.Log.Info(Utils.LogTag, $"[{e.Message}]  {sender.ToString()} State is Paused");
                    break;

                case AnimatorState.Stopped:
                    Tizen.Log.Info(Utils.LogTag, $"[{e.Message}]  {sender.ToString()} State is Stopped");
                    break;

                case AnimatorState.AnimationFinished:
                    Tizen.Log.Info(Utils.LogTag, $"[{e.Message}]  {sender.ToString()} Play Completed");
                    break;
            }
        }

    }
}
