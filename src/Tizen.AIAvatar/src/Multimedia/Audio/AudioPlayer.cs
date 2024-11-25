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
using System.IO;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.Multimedia;

using static Tizen.AIAvatar.AIAvatar;

namespace Tizen.AIAvatar
{
    public class AudioPlayer
    {

        private int accLength = 0;
        private int streamIndex = 0;

        private bool isStreaming = false;

        private MemoryStream audioStream;
        private MemoryStream baseAudioStream;
        private List<MemoryStream> streamList;

        private AudioDucking audioDucking;
        private AudioPlayback audioPlayback;
        private AudioStreamPolicy audioStreamPolicy;

        private Timer bufferChecker;

        private AudioPlayerState currentAudioPlayerState = AudioPlayerState.Unavailable;
        internal event EventHandler<AudioPlayerChangedEventArgs> AudioPlayerStateChanged;



        public AudioPlayer(AudioOptions audioOptions = null)
        {
            
            if (audioOptions == null)
                CurrentAudioOptions = DefaultAudioOptions;
            else
                CurrentAudioOptions = audioOptions;

            
            baseAudioStream = new MemoryStream();
            streamList = new List<MemoryStream>();

            audioStreamPolicy = new AudioStreamPolicy(CurrentAudioOptions.StreamType);
            InitAudio(CurrentAudioOptions.SampleRate);

            bufferChecker = new Timer(100);
            bufferChecker.Tick += OnBufferChecker;

            audioDucking = new AudioDucking(CurrentAudioOptions.DuckingTargetStreamType);
            audioDucking.DuckingStateChanged += (sender, arg) =>
            {
                if (arg.IsDucked)
                {
                    CurrentAudioPlayerState = AudioPlayerState.Playing;
                }
            };

            AudioPlayerStateChanged += OnStateChanged;
        }

        public void InitializeStream()
        {
            isStreaming = true;
            streamIndex = 0;
            streamList.Clear();
        }

        public void AddStream(byte[] buffer)
        {
            streamList.Add(new MemoryStream(buffer));
        }

        public bool IsPrepare()
        {
            return streamList.Count > 0;
        }

        public void PlayStreamAudio(int sampleRate = 0)
        {            
            InitializeStream();

            if (audioPlayback == null || audioPlayback.SampleRate != sampleRate)
            {
                InitAudio(sampleRate);
            }


            try
            {
                audioDucking.Activate(AIAvatar.CurrentAudioOptions.DuckingDuration, AIAvatar.CurrentAudioOptions.DuckingRatio);
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to PlayAsync AudioPlayback. {e.Message}");
                CurrentAudioPlayerState = AudioPlayerState.Playing;
            }
        }

        public void Play(byte[] audioBytes, int sampleRate = 0)
        {
            isStreaming = false;

            if (audioBytes == null)
            {
                Log.Error(LogTag, $"Play AudioPlayBack null.");
                return;
            }

            if (audioPlayback.SampleRate != sampleRate)
            {
                InitAudio(sampleRate);
            }

            InitializeStream();
            streamList.Add(new MemoryStream(audioBytes));

            try
            {
                audioDucking.Activate(CurrentAudioOptions.DuckingDuration, CurrentAudioOptions.DuckingRatio);
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to Play AudioPlayback. {e.Message}");
                CurrentAudioPlayerState = AudioPlayerState.Playing;
            }
        }

        public void Pause()
        {
            CurrentAudioPlayerState = AudioPlayerState.Paused;

        }

        public void Stop()
        {
            CurrentAudioPlayerState = AudioPlayerState.Stopped;
        }

        public void Destroy()
        {
            DestroyAudioPlayback();
            streamList.Clear();
            streamList = null;
        }

        public AudioPlayerState CurrentAudioPlayerState
        {
            get => currentAudioPlayerState;
            protected set
            {
                if (currentAudioPlayerState == value) return;

                var preState = currentAudioPlayerState;
                currentAudioPlayerState = value;

                AudioPlayerStateChanged?.Invoke(this, new AudioPlayerChangedEventArgs(preState, currentAudioPlayerState));
            }
        }


        private bool OnBufferChecker(object source, Timer.TickEventArgs e)
        {
            if (isStreaming && streamList.Count == 0)
            {
                return true;
            }

            if (audioStream != null && audioStream.Position == audioStream.Length)
            {
                if (streamIndex >= streamList.Count)
                {
                    CurrentAudioPlayerState = AudioPlayerState.Finished;
                    Log.Debug(LogTag, $"Complete Play Audio Buffer.");
                    return false;
                }
            }

            return true;
        }

        private void OnStateChanged(object sender, AudioPlayerChangedEventArgs state)
        {
            try
            {
                switch (state.Current)
                {
                    case AudioPlayerState.Playing:
                        Log.Debug(LogTag, "Audio is playing.");

                        bufferChecker?.Start();
                        audioPlayback?.Prepare();

                        break;

                    case AudioPlayerState.Paused:
                        Log.Debug(LogTag, "Audio is paused.");

                        bufferChecker?.Stop();
                        audioPlayback?.Pause();
                        audioPlayback?.Unprepare();

                        if (audioDucking.IsDucked)
                            audioDucking?.Deactivate();


                        break;

                    case AudioPlayerState.Stopped:
                    case AudioPlayerState.Finished:
                        Log.Debug(LogTag, "Audio is stopped.");

                        bufferChecker?.Stop();
                        audioPlayback?.Pause();
                        audioPlayback?.Unprepare();

                        if (audioDucking.IsDucked)
                            audioDucking?.Deactivate();

                        streamIndex = 0;
                        audioStream = baseAudioStream;

                        break;
                }
            }
            catch (Exception e)
            {
                //Log.Error(LogTag, $"Failed to StateChanged. {e.Message}");
            }
        }

        private void OnBufferAvailable(object sender, AudioPlaybackBufferAvailableEventArgs args)
        {
            if (audioStream.Position == audioStream.Length)
            {

                if (streamIndex < streamList.Count)
                {
                    audioStream = streamList[streamIndex];
                    accLength = (int)audioStream.Length;

                    streamIndex++;
                }
                else
                {
                    return;
                }
            }

            try
            {
                if (args.Length > 1024)
                {
                    accLength -= args.Length;
                    int length = args.Length;
                    if (accLength < 0)
                    {
                        length += accLength;
                    }

                    var buffer = new byte[length];
                    audioStream.Read(buffer, 0, length);
                    audioPlayback.Write(buffer);
                }
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to write. {e.Message}");
            }
        }

        private void InitAudio(int sampleRate)
        {
            if (audioPlayback != null)
            {
                DestroyAudioPlayback();
            }
            if (sampleRate == 0)
            {
                sampleRate = CurrentAudioOptions.SampleRate;
            }

            try
            {
                audioPlayback = new AudioPlayback(sampleRate, CurrentAudioOptions.Channel, CurrentAudioOptions.SampleType);
                audioPlayback.ApplyStreamPolicy(audioStreamPolicy);
                audioPlayback.BufferAvailable += OnBufferAvailable;

                audioStream = baseAudioStream;
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to create AudioPlayback. {e.Message}");
            }

        }

        private void DestroyAudioPlayback()
        {
            if (audioPlayback != null)
            {
                Stop();
                audioPlayback.BufferAvailable -= OnBufferAvailable;
                audioPlayback.Dispose();
            }

            audioPlayback = null;
        }


    }
}
