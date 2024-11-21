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

        public event EventHandler<TtsStreamingEventArgs> OnTtsStart;
        public event EventHandler<TtsStreamingEventArgs> OnTtsReceiving;
        public event EventHandler<TtsStreamingEventArgs> OnTtsFinish;

        public override string ServiceName => "OpenAI";
        public override ServiceCapabilities Capabilities =>
            ServiceCapabilities.TextToSpeech | ServiceCapabilities.LargeLanguageModel;

        public OpenAIService(OpenAIConfiguration config) : base(config)
        {
            this.config = config;
        }

        public async Task<string> GenerateTextAsync(
            string prompt,
            Dictionary<string, object> options = null)
        {
            var client = ClientManager.GetClient(config.Endpoints.LLMEndpoint);
            var messages = new List<object>
        {
            new { role = "user", content = prompt }
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

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new Exception($"OpenAI API Error: {response.ErrorMessage}");

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);
            return jsonResponse
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
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
            const int BYTES_PER_SAMPLE = 2;  // 16-bit PCM
            const int CHUNK_SIZE = (SAMPLE_RATE * FRAME_DURATION_MS * BYTES_PER_SAMPLE) / 1000;  // 160ms worth of PCM data

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
                OnTtsStart?.Invoke(this, new TtsStreamingEventArgs
                {
                    Text = text,
                    Voice = voice ?? "alloy",
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
                    var currentChunkSize = Math.Min(CHUNK_SIZE, remainingBytes);
                    var chunk = new byte[currentChunkSize];
                    Array.Copy(audioData, bytesProcessed, chunk, 0, currentChunkSize);
                    bytesProcessed += currentChunkSize;

                    // 현재 청크의 진행 정보와 함께 오디오 데이터를 이벤트로 전달
                    OnTtsReceiving?.Invoke(this, new TtsStreamingEventArgs
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

                OnTtsFinish?.Invoke(this, new TtsStreamingEventArgs
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
                OnTtsFinish?.Invoke(this, new TtsStreamingEventArgs
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
