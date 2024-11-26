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
using System.Text.Json;
using System.Threading.Tasks;

namespace Tizen.AIAvatar
{
    public class OpenAIConfiguration : AIServiceConfiguration
    {
        public OpenAIConfiguration()
        {
            Endpoints = new ServiceEndpoints
            {
                LLMEndpoint = "https://api.openai.com/v1/chat/",
                TextToSpeechEndpoint = "https://api.openai.com/v1/audio/"
            };
        }

        public string Model { get; set; } = "gpt-3.5-turbo";
        public string Organization { get; set; }
    }

    public class OpenAIService : BaseAIService, ITextToSpeechService, ILLMService
    {
        private readonly OpenAIConfiguration config;

        public event EventHandler<llmResponseEventArgs> ResponseHandler;

        public event EventHandler<ttsStreamingEventArgs> OnTtsStart;
        public event EventHandler<ttsStreamingEventArgs> OnTtsReceiving;
        public event EventHandler<ttsStreamingEventArgs> OnTtsFinish;

        public override string ServiceName => "OpenAI";
        public override ServiceCapabilities Capabilities =>
            ServiceCapabilities.TextToSpeech | ServiceCapabilities.LargeLanguageModel;

        public OpenAIService(OpenAIConfiguration config) : base(config)
        {
            this.config = config;
        }

        public async Task GenerateTextAsync(string message, Dictionary<string, object> options = null)
        {
            var client = ClientManager.GetClient(config.Endpoints.LLMEndpoint);
            var messages = new List<object>
        {
            new { role = "user", content = message }
        };

            var request = new RestRequest("completions", Method.Post)
                .AddHeader("Authorization", $"Bearer {config.ApiKey}")
                .AddJsonBody(new
                {
                    model = config.Model,
                    messages = messages,
                    temperature = options?.GetValueOrDefault("temperature", 0.7),
                    max_tokens = options?.GetValueOrDefault("max_tokens", 1000)
                });

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                ResponseHandler?.Invoke(this, new llmResponseEventArgs { Error = response.ErrorMessage });
                return;
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);
            string content = jsonResponse
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();

            ResponseHandler?.Invoke(this, new llmResponseEventArgs { Text = content });
        }

        public async Task<byte[]> TextToSpeechAsync(
            string text,
            string voice = null,
            Dictionary<string, object> options = null)
        {
            var client = ClientManager.GetClient(config.Endpoints.TextToSpeechEndpoint);

            var request = new RestRequest("speech", Method.Post)
                .AddHeader("Authorization", $"Bearer {config.ApiKey}")
                .AddJsonBody(new
                {
                    model = "tts-1",
                    input = text,
                    voice = voice ?? "alloy",
                    response_format = "mp3"
                });

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception($"OpenAI TTS Error: {response.ErrorMessage}");

            return response.RawBytes;
        }

        public async Task TextToSpeechStreamAsync(string text, string voice = null, Dictionary<string, object> options = null)
        {
            const int SAMPLE_RATE = 24000;  // OpenAI TTS의 기본 샘플레이트
            const int FRAME_DURATION_MS = 160;  // 160ms 단위로 분할
            const int TAIL_DURATION_MS = 15;
            const int BYTES_PER_SAMPLE = 2;  // 16-bit PCM
            const int CHUNK_SIZE = (SAMPLE_RATE * FRAME_DURATION_MS * BYTES_PER_SAMPLE) / 1000;  // 160ms worth of PCM data
            const int TAIL_CHUNK_SIZE = (SAMPLE_RATE * TAIL_DURATION_MS * BYTES_PER_SAMPLE) / 1000;

            var client = ClientManager.GetClient(config.Endpoints.TextToSpeechEndpoint);
            var request = new RestRequest("speech", Method.Post)
                .AddHeader("Authorization", $"Bearer {config.ApiKey}")
                .AddJsonBody(new
                {
                    model = "tts-1",
                    input = text,
                    voice = voice ?? "alloy",
                    response_format = "pcm",  // PCM 포맷으로 변경
                    //speed = options?.GetValueOrDefault("speed", 1.0)
                });

            try
            {
                OnTtsStart?.Invoke(this, new ttsStreamingEventArgs
                {
                    Text = text,
                    Voice = voice ?? "alloy",
                    SampleRate = SAMPLE_RATE,
                    TotalBytes = 0,
                    AudioData = Array.Empty<byte>()
                });

                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"OpenAI TTS Error: {response.ErrorMessage}");
                }

                var audioData = response.RawBytes;
                var totalBytes = audioData.Length;
                var bytesProcessed = 0;

                // Process the audio data in 160ms chunks
                while (bytesProcessed < totalBytes)
                {
                    var remainingBytes = totalBytes - bytesProcessed;
                    var currentChunkSize = Math.Min(CHUNK_SIZE + TAIL_CHUNK_SIZE, remainingBytes);
                    var chunk = new byte[currentChunkSize];
                    Array.Copy(audioData, bytesProcessed, chunk, 0, currentChunkSize);
                    bytesProcessed += Math.Min(CHUNK_SIZE, remainingBytes);

                    // 현재 청크의 진행 정보와 함께 오디오 데이터를 이벤트로 전달
                    OnTtsReceiving?.Invoke(this, new ttsStreamingEventArgs
                    {
                        Text = text,
                        Voice = voice ?? "alloy",
                        TotalBytes = totalBytes,
                        ProcessedBytes = bytesProcessed,
                        ProgressPercentage = (double)bytesProcessed / totalBytes * 100,
                        AudioData = chunk
                    });

                    // 160ms 간격을 시뮬레이션하기 위한 딜레이 (실제 스트리밍 시나리오에서 필요한 경우)
                    // await Task.Delay(FRAME_DURATION_MS);
                }

                OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                {
                    Text = text,
                    Voice = voice ?? "alloy",
                    TotalBytes = totalBytes,
                    ProcessedBytes = totalBytes,
                    ProgressPercentage = 100,
                    AudioData = Array.Empty<byte>()
                });
            }
            catch (Exception ex)
            {
                OnTtsFinish?.Invoke(this, new ttsStreamingEventArgs
                {
                    Text = text,
                    Voice = voice ?? "alloy",
                    Error = ex.Message,
                    AudioData = Array.Empty<byte>()
                });
                throw;
            }
        }
    }
}
