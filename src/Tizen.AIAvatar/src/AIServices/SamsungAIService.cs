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
        }

        public async Task GenerateTextAsync(string prompt, Dictionary<string, object> options = null)
        {
            var client = ClientManager.GetClient(config.Endpoints.LLMEndpoint);
            var messages = new List<object>
    {
        new { role = "user", content = prompt }
    };

            var request = new RestRequest(Method.Post)
                .AddHeader("Authorization", $"Bearer {config.ApiKey}")
                .AddJsonBody(new
                {
                    model = config.Model,
                    messages = messages,
                    temperature = options?["temperature"] ?? 0.5,
                    seed = options?.GetValueOrDefault("seed", 0)
                });

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                ResponseHandler?.Invoke(this, new llmResponseEventArgs { Error = response.ErrorMessage });
                return;
            }

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);
            string content = jsonResponse.GetProperty("response").GetProperty("content").GetString();

            ResponseHandler?.Invoke(this, new llmResponseEventArgs { Text = content });
        }

        public async Task<byte[]> TextToSpeechAsync(
            string text,
            string voice = null,
            Dictionary<string, object> options = null)
        {

            return null;
        }

        public async Task TextToSpeechStreamAsync(string text, string voice = null, Dictionary<string, object> options = null)
        {

        }

        private void OnResponseHandler(llmResponseEventArgs e)
        {
            ResponseHandler?.Invoke(this, e);
        }
    }
}
