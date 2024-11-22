using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tizen;
using Tizen.AIAvatar;
using Tizen.NUI.Scene3D;

namespace AIAvatar
{
    public class MyEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public partial class AIAvatar : Model
    {

        private SamsungAIService samsungAIService;
        private SamsungAIConfiguration samsungAIConfig;



        public void InitializeAIServices()
        {
            samsungAIConfig = new SamsungAIConfiguration
            {
                ApiKey = "API_KEY",
                Model = "chat-65b-32k-1.1.2"
            };

            samsungAIService = new SamsungAIService(samsungAIConfig);
            samsungAIService.ResponseHandler += HandleResponse;
        }

        public void TestSamsungAIService()
        {
            TestSamsungTextGeneration(samsungAIService);
            //await TestOpenAITTS(openAIService);           
        }

        #region SamsungAI Services

        private void TestSamsungTextGeneration(SamsungAIService samsungAIService)
        {
            var task = Task.Run(async () =>
            {
                await samsungAIService.GenerateTextAsync("hello?");
            });


        }

        #endregion


        public void TestOpenAIServicesAsync()
        {
            var openAIConfig = new OpenAIConfiguration
            {
                ApiKey = "API_KEY",
                Model = "gpt-4o"
            };

            using (var openAIService = new OpenAIService(openAIConfig))
            {
                var task = Task.Run(async () =>
                {
                    await TestOpenAITextGeneration(openAIService);
                    await TestOpenAITTS(openAIService);
                });
            }
        }


        #region OpenAI Services


        private async Task TestOpenAITextGeneration(OpenAIService openAIService)
        {
            await openAIService.GenerateTextAsync("안녕?");
        }

        private async Task TestOpenAITTS(OpenAIService openAIService)
        {
            // Basic TTS test
            var speechBytes = await openAIService.TextToSpeechAsync(
                "Hello from OpenAI!", "alloy");
            await File.WriteAllBytesAsync("openai_speech.mp3", speechBytes);

            // Streaming TTS test
            AudioProcessor audioProcessor = new AudioProcessor();
            string outputPath = System.IO.Path.Combine(Utils.ResourcePath, "OpenAI_TTS.wav");


            openAIService.OnTtsStart += (sender, e) =>
             Log.Info(Utils.LogTag, $"Started TTS for text: {e.Text}");

            openAIService.OnTtsReceiving += (sender, e) =>
            {
                Log.Info(Utils.LogTag, $"Progress: {e.ProgressPercentage:F2}%");
                audioProcessor.ProcessAudioChunk(e.AudioData);
            };

            openAIService.OnTtsFinish += async (sender, e) =>
            {
                Log.Info(Utils.LogTag, "TTS streaming completed");
                await audioProcessor.SaveToFileAsync(outputPath);
                audioProcessor.Dispose();
            };


            await openAIService.TextToSpeechStreamAsync(
                "Hello from OpenAI!, I'm so excited to go on vacation!", "alloy");
        }
        #endregion

        public void TestGoogleAIServices()
        {

            var googleAIConfig = new GoogleAIConfiguration
            {
                ApiKey = "API_KEY",
                OAuth2Token = "TOKEN_API_KEY",
                Model = "gemini-1.5-flash"
            };

            using (var googleAIService = new GoogleAIService(googleAIConfig))
            {
                var task = Task.Run(async () =>
                {
                    await TestGoogleTextGeneration(googleAIService);
                    await TestGoogleTTS(googleAIService);
                    await TestGoogleSTT(googleAIService);
                });
            }
        }

        #region Google AI Services

        private async Task TestGoogleTextGeneration(GoogleAIService googleAIService)
        {
            var options = new Dictionary<string, object>
        {
            { "temperature", 1.0 },
            { "maxOutputTokens", 8192 },
            { "topP", 0.95 },
            { "topK", 40 }
        };

            await googleAIService.GenerateTextAsync("hello?", options);
        }

        private async Task TestGoogleTTS(GoogleAIService googleAIService)
        {
            // Basic TTS test
            var speechBytes = await googleAIService.TextToSpeechAsync(
                "Hello from Google AI!", "en-US-Standard-A");

            AudioProcessor audioProcessor = new AudioProcessor();
            string outputPath = System.IO.Path.Combine(Utils.ResourcePath, "GoogleTTS.wav");


            googleAIService.OnTtsStart += (sender, e) =>
                Log.Info(Utils.LogTag, $"Started TTS for text: {e.Text}");

            googleAIService.OnTtsReceiving += (sender, e) =>
            {
                Log.Info(Utils.LogTag, $"Progress: {e.ProgressPercentage:F2}%");
                audioProcessor.ProcessAudioChunk(e.AudioData);
            };

            googleAIService.OnTtsFinish += async (sender, e) =>
            {
                Log.Info(Utils.LogTag, "TTS streaming completed");
                await audioProcessor.SaveToFileAsync(outputPath);
                audioProcessor.Dispose();
            };


            var tts_options = new Dictionary<string, object>
            {
                ["audioEncoding"] = "LINEAR16",
                ["sampleRate"] = 24000,
            };

            await googleAIService.TextToSpeechStreamAsync(
                "Hello from Google!, I'm so excited to go on vacation!",
                "en-US-Standard-A",
                tts_options
            );
        }

        private async Task TestGoogleSTT(GoogleAIService googleAIService)
        {
            var stt_options = new Dictionary<string, object>
            {
                ["encoding"] = "LINEAR16",
                ["sampleRate"] = 24000,
                ["languageCode"] = "en-US",
                ["enablePunctuation"] = true
            };

            WaveData wavData = AudioUtils.LoadWave("GoogleTTS.wav");
            string result = await googleAIService.SpeechToTextAsync(wavData.RawAudioData, stt_options);
            Log.Info(Utils.LogTag, $"변환된 텍스트: {result}");
        }
        #endregion

        private void HandleResponse(object sender, llmResponseEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Info(Utils.LogTag, $"Error: {e.Error}");
                return;
            }
                        
            Log.Info(Utils.LogTag, $"Response: {e.Text}");
        }

    }
}
