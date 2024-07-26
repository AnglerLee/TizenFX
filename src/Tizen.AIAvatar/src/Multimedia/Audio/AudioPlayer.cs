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

using System.Linq;
using System.Collections.Generic;
using Tizen.Multimedia;
using System.IO;
using System;

using static Tizen.AIAvatar.AIAvatar;

namespace Tizen.AIAvatar
{
    internal class AudioPlayer
    {
        private AudioPlayback audioPlayback;
        private MemoryStream audioStream;
        private List<MemoryStream> streamList;
        private int streamIndex = 0;
        int accLength = 0;

        internal AudioPlayer()
        {
            if (streamList == null)
            {
                streamList = new List<MemoryStream>();
            }
        }

        internal void InitStreamList()
        {
            streamList.Clear();
        }

        internal void AddStreamList(byte[] buffer)
        {
            var audioStream = new MemoryStream(buffer);
            streamList.Add(audioStream);
        }

        internal bool IsPrepareSound(int minBufferSize = 0)
        {
            return streamList.Count > minBufferSize;
        }

        internal void PlayAsync(int sampleRate = 0)
        {
            if (audioPlayback != null)
            {
                return;
            }

            if (streamList.Count <= 0)
            {
                Tizen.Log.Error(LogTag, "StreamList Count is 0");
                return;
            }

            try
            {
                InitAudio(sampleRate);
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to create AudioPlayback. {e.Message}");
                return;
            }
                        
            if (audioPlayback != null)
            {
                audioPlayback.Prepare();
                audioStream = streamList[streamIndex];
                accLength = (int)audioStream.Length;
              
                audioPlayback.BufferAvailable += (sender, args) =>
                {
                    if (audioStream.Position == audioStream.Length)
                    {
                        streamIndex++;
                        if (streamIndex >= streamList.Count)
                        {
                            return;
                        }

                        audioStream = streamList[streamIndex];
                        accLength = (int)audioStream.Length;
                    }

                    try
                    {
                        if (args.Length > 0)
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
                };
            }
        }

        internal void Play(byte[] audioBytes, int sampleRate = 0)
        {
            if (audioBytes == null)
            {
                return;
            }

            try
            {
                InitAudio(sampleRate);
                streamList = new List<MemoryStream>();
            }
            catch (Exception e)
            {
                Log.Error(LogTag, $"Failed to create AudioPlayback. {e.Message}");
                return;
            }

            if (audioPlayback != null)
            {
                audioPlayback.Prepare();

                audioStream = new MemoryStream(audioBytes);
                accLength = (int)audioStream.Length;

                audioPlayback.BufferAvailable += (sender, args) =>
                {
                    if (audioStream.Position == audioStream.Length)
                    {
                        return;
                    }

                    try
                    {
                        if (args.Length > 0)
                        {
                            var buffer = new byte[args.Length];
                            audioStream.Read(buffer, 0, args.Length);
                            audioPlayback.Write(buffer);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(LogTag, $"Failed to write. {e.Message}");
                    }
                };
            }
        }

        internal void Pause()
        {
            if (audioPlayback != null)
            {
                audioPlayback.Pause();
            }
            else
            {
                Log.Error(LogTag, $"audioPlayBack is null");
            }
        }

        internal void Stop()
        {
            if (audioPlayback != null)
            {
                streamList.Clear();
                audioPlayback.Pause();
                DestroyAudioPlayback();
            }
            else
            {
                Log.Error(LogTag, $"audioPlayBack is null");
            }
        }

        internal void Destroy()
        {
            DestroyAudioPlayback();
            streamList.Clear();
            streamList = null;
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

            streamIndex = 0;

            audioPlayback = new AudioPlayback(sampleRate, CurrentAudioOptions.Channel, CurrentAudioOptions.SampleType);
        }

        private void DestroyAudioPlayback()
        {
            audioPlayback?.Unprepare();
            audioPlayback?.Dispose();
            audioPlayback = null;
        }
    }
}
