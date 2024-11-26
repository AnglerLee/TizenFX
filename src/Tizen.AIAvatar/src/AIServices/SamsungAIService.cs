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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tizen.AIAvatar.Samsung;
using Tizen.Uix.Tts;


namespace Tizen.AIAvatar
{
    public class SamsungAIConfiguration : AIServiceConfiguration
    {
        public SamsungAIConfiguration()
        {
            Endpoints = new ServiceEndpoints
            {
                LLMEndpoint = "https://playground-api.sec.samsung.net/api/v1/chat/completions",
                TextToSpeechEndpoint = "OnDevice"
            };
        }

        public string Model { get; set; } = "chat-65b-32k-1.1.2";
        public string SmallModel { get; set; } = "Gauss.L-1B-0.1.1";
    }

    public class SamsungAIService : BaseAIService, ITextToSpeechService, ILLMService
    {
        private readonly SamsungAIConfiguration config;

        private readonly TtsClient ttsHandle;
        private TaskCompletionSource<bool> ttsCompletionSource;
                
                
        private const float audioLengthFactor = 0.16f;
        private const float audioTailLengthFactor = 0.015f;
        private const float audioBufferMultiflier = 2f;

        private int audioLength;
        private int desiredBufferLength;
        private int audioTailLength;

        private byte[] recordedBuffer;
        private byte[] audioMainBuffer;

        private float desiredBufferDuration = audioLengthFactor + audioTailLengthFactor;
        

        public event EventHandler<llmResponseEventArgs> ResponseHandler;

        public event EventHandler<ttsStreamingEventArgs> OnTtsStart;
        public event EventHandler<ttsStreamingEventArgs> OnTtsReceiving;
        public event EventHandler<ttsStreamingEventArgs> OnTtsFinish;
        

        public override string ServiceName => "SamsungResearch";
        public override ServiceCapabilities Capabilities =>
            ServiceCapabilities.TextToSpeech | ServiceCapabilities.LargeLanguageModel;

        public SamsungAIService(SamsungAIConfiguration config) : base(config)
        {
            this.config = config;

            try
            {
                ttsHandle = new TtsClient();
                ttsHandle.SynthesizedPcm += TtsSynthesizedPCM;
                ttsHandle.PlayingMode = PlayingMode.ByClient;

                ttsHandle.Prepare();

                GetSupportedVoices();
            }
            catch (Exception e)
            {
                throw new Exception($"[ERROR] Failed to prepare TTS {e.Message}");
            }
        }

        public async Task GenerateTextAsync(string message, Dictionary<string, object> options = null)
        {
            var client = ClientManager.GetClient(config.Endpoints.LLMEndpoint);

            var request = new RestRequest(Method.Post)
                .AddHeader("Authorization", $"Bearer {config.ApiKey}");

            int taskID = (int)(options?["TaskID"] ?? 0);

            if (options != null && options.TryGetValue("jsonFilePath", out var jsonFilePathObj) && jsonFilePathObj is string jsonFilePath)
            {
                // Read JSON file content
                if (File.Exists(jsonFilePath))
                {
                    var jsonContent = await File.ReadAllTextAsync(jsonFilePath).ConfigureAwait(false);
                    Prompt prompt = JsonSerializer.Deserialize<Prompt>(jsonContent);
                    var msg = prompt.messages.Last();
                    msg.content = String.Format(msg.content, message);

                    request.AddJsonBody(prompt);
                }
                else
                {
                    ResponseHandler?.Invoke(this, new llmResponseEventArgs { TaskID = taskID, Error = $"File not found: {jsonFilePath}" });
                    return;
                }
            }
            else
            {

                var messages = new List<object>
                {
                    new { role = "user", content = message }
                };
                // Add the default body if no JSON file is provided
                request.AddJsonBody(new
                {
                    model = config.Model,
                    messages = messages,
                    temperature = options?["temperature"] ?? 0.5,
                    seed = options?.GetValueOrDefault("seed", 0)
                });
            }

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                ResponseHandler?.Invoke(this, new llmResponseEventArgs { TaskID = taskID, Error = response.ErrorMessage });
                return;
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);
            string content = jsonResponse.GetProperty("response").GetProperty("content").GetString();

            ResponseHandler?.Invoke(this, new llmResponseEventArgs { TaskID = taskID, Text = content });
        }

        public async Task<byte[]> TextToSpeechAsync(
            string text,
            string voice = null,
            Dictionary<string, object> options = null)
        {

            return null;
        }

        public async Task TextToSpeechStreamAsync(string text, string voice, Dictionary<string, object> options)
        {
            ttsCompletionSource = new TaskCompletionSource<bool>();
            recordedBuffer = Array.Empty<byte>();

            //Option 처리
            SpeedRange speedRange = ttsHandle.GetSpeedRange();
            int speed = speedRange.Normal;
            int voiceType = (int)VoiceType.Auto;

            if (options != null)
            {
                if (options.ContainsKey("speechRate"))
                {
                    float speechRate = (float)(options["speechRate"]) / 2.0f;
                    speed = (int)(speedRange.Min + (speedRange.Max - speedRange.Min) * speechRate);
                }

                if (options.ContainsKey("voiceType"))
                {
                    voiceType = (int)options["voiceType"];
                }
            }
            /////////
            try
            {
                ttsHandle.AddText(text, voice, voiceType, speed);

                ttsHandle.Play();

                await ttsCompletionSource.Task;
            }
            catch (Exception ex)
            {
                OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                {
                    Text = text,
                    Voice = voice,
                    Error = ex.Message,
                    AudioData = Array.Empty<byte>()
                });
            }
        }

        private void TtsSynthesizedPCM(object sender, SynthesizedPcmEventArgs e)
        {
            try
            {
                switch (e.EventType)
                {
                    case SynthesizedPcmEvent.Start:
                        recordedBuffer = Array.Empty<byte>();

                        audioLength = (int)(audioLengthFactor * e.SampleRate * audioBufferMultiflier);
                        audioTailLength = (int)(audioTailLengthFactor * e.SampleRate  * audioBufferMultiflier);
                        desiredBufferLength = (int)(desiredBufferDuration * e.SampleRate * audioBufferMultiflier);

                        audioMainBuffer = new byte[desiredBufferLength];

                        OnTtsStart?.Invoke(this, new ttsStreamingEventArgs
                        {
                            SampleRate = e.SampleRate,
                            AudioBytes = audioLength,
                            TotalBytes = 0,
                            AudioData = Array.Empty<byte>()
                        });

                        Log.Info("Tizen.AIAvatar", $"TTS Start: UtteranceId={e.UtteranceId}, SampleRate={e.SampleRate}");

                        break;

                    case SynthesizedPcmEvent.Continue:

                        recordedBuffer = recordedBuffer.Concat(e.Data).ToArray();

                        if (recordedBuffer.Length >= desiredBufferLength)
                        {
                            Buffer.BlockCopy(recordedBuffer, 0, audioMainBuffer, 0, desiredBufferLength);
                            OnTtsReceiving?.Invoke(this, new ttsStreamingEventArgs
                            {
                                AudioData = audioMainBuffer,
                                ProcessedBytes = audioMainBuffer.Length
                            });

                            
                            int slicedBufferLength = recordedBuffer.Length - audioLength;
                            byte[] slicedBuffer = new byte[slicedBufferLength];
                            Buffer.BlockCopy(recordedBuffer, audioLength, slicedBuffer, 0, slicedBufferLength);

                            recordedBuffer = slicedBuffer;
                        }

                        break;

                    case SynthesizedPcmEvent.Finish:
                        Log.Info("Tizen.AIAvatar", $"TTS Finish: UtteranceId={e.UtteranceId}");

                        // Send any remaining audio data
                        if (recordedBuffer.Length > 0)
                        {
                            int minBufferSize = Math.Min(desiredBufferLength, recordedBuffer.Length);

                            Array.Clear(audioMainBuffer, 0, desiredBufferLength);
                            Buffer.BlockCopy(recordedBuffer, 0, audioMainBuffer, 0, minBufferSize);
                            OnTtsReceiving?.Invoke(this, new ttsStreamingEventArgs
                            {
                                AudioData = audioMainBuffer,
                                ProcessedBytes = minBufferSize,
                                ProgressPercentage = 100
                            });
                        }

                        OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                        {
                            AudioData = Array.Empty<byte>(),
                            TotalBytes = recordedBuffer.Length,
                            ProgressPercentage = 100
                        });

                        ttsCompletionSource?.SetResult(true);
                        break;

                    case SynthesizedPcmEvent.Fail:
                        var error = "TTS synthesis failed";
                        
                        OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                        {
                            Error = error,
                            AudioData = Array.Empty<byte>()
                        });

                        ttsCompletionSource?.SetException(new Exception(error));

                        break;

                }
            }
            catch (Exception ex)
            {
                Log.Error("Tizen.AIAvatar", $"Error in TtsSynthesizedPCM: {ex.Message}");
                OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                {
                    Error = ex.Message,
                    AudioData = Array.Empty<byte>()
                });
                ttsCompletionSource?.SetException(ex);
            }
        }

        private List<VoiceInfo> GetSupportedVoices()
        {
            var voiceInfoList = new List<VoiceInfo>();

            if (ttsHandle == null)
            {
                Log.Error("Tizen.AIAvatar", $"ttsHandle is null");
                return voiceInfoList;
            }

            var supportedVoices = ttsHandle.GetSupportedVoices();
            foreach (var supportedVoice in supportedVoices)
            {
                Log.Info("Tizen.AIAvatar", $"{supportedVoice.Language} & {supportedVoice.VoiceType} is supported");
                voiceInfoList.Add(new VoiceInfo() { Language = supportedVoice.Language, Type = (VoiceType)supportedVoice.VoiceType });
            }
            return voiceInfoList;
        }
        private void OnResponseHandler(llmResponseEventArgs e)
        {
            ResponseHandler?.Invoke(this, e);
        }
    }
}
