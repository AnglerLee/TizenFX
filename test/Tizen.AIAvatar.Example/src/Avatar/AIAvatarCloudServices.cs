using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tizen;
using Tizen.AIAvatar;
using Tizen.NUI;
using Tizen.NUI.Scene3D;

namespace AIAvatar
{
    public partial class AIAvatar : Model
    {
        private static async Task TestOpenAIServices()
        {
            var openAIConfig = new OpenAIConfiguration
            {
                ApiKey = "API_KEY",
                Model = "gpt-4o"
            };

            using (var openAIService = new OpenAIService(openAIConfig))
            {
                await TestOpenAITextGeneration(openAIService);
                await TestOpenAITTS(openAIService);
            }
        }


        #region OpenAI Services


        private static async Task TestOpenAITextGeneration(OpenAIService openAIService)
        {
            var generatedText = await openAIService.GenerateTextAsync("안녕?");
            Console.WriteLine("Generated Text:");
            Console.WriteLine(generatedText);
        }

        private static async Task TestOpenAITTS(OpenAIService openAIService)
        {
            // Basic TTS test
            var speechBytes = await openAIService.TextToSpeechAsync(
                "Hello from OpenAI!", "alloy");
            await File.WriteAllBytesAsync("openai_speech.mp3", speechBytes);

            // Streaming TTS test
            AudioProcessor audioProcessor = new AudioProcessor();
            string outputPath = System.IO.Path.Combine(Utils.ResourcePath, "OpenAI_TTS.wav");


            openAIService.OnTtsStart += (sender, e) =>
             Console.WriteLine($"Started TTS for text: {e.Text}");

            openAIService.OnTtsReceiving += (sender, e) =>
            {
                Console.WriteLine($"Progress: {e.ProgressPercentage:F2}%");
                audioProcessor.ProcessAudioChunk(e.AudioData);
            };

            openAIService.OnTtsFinish += async (sender, e) =>
            {
                Console.WriteLine("TTS streaming completed");
                await audioProcessor.SaveToFileAsync(outputPath);
                audioProcessor.Dispose();
            };


            await openAIService.TextToSpeechStreamAsync(
                "Hello from OpenAI!, I'm so excited to go on vacation!", "alloy");
        }
        #endregion

        private static async Task TestGoogleAIServices()
        {

            var googleAIConfig = new GoogleAIConfiguration
            {
                ApiKey = "API_KEY",
                OAuth2Token = "TOKEN_API_KEY",
                Model = "gemini-1.5-flash"
            };

            using (var googleAIService = new GoogleAIService(googleAIConfig))
            {
                await TestGoogleTextGeneration(googleAIService);
                await TestGoogleTTS(googleAIService);
                await TestGoogleSTT(googleAIService);
            }
        }

        #region Google AI Services

        private static async Task TestGoogleTextGeneration(GoogleAIService googleAIService)
        {
            var options = new Dictionary<string, object>
        {
            { "temperature", 1.0 },
            { "maxOutputTokens", 8192 },
            { "topP", 0.95 },
            { "topK", 40 }
        };

            var generatedText = await googleAIService.GenerateTextAsync("hello?", options);
            Console.WriteLine("Generated Text:");
            Console.WriteLine(generatedText);
        }

        private static async Task TestGoogleTTS(GoogleAIService googleAIService)
        {
            // Basic TTS test
            var speechBytes = await googleAIService.TextToSpeechAsync(
                "Hello from Google AI!", "en-US-Standard-A");
                        
            AudioProcessor audioProcessor = new AudioProcessor();
            string outputPath = System.IO.Path.Combine(Utils.ResourcePath, "GoogleTTS.wav");
                    

            googleAIService.OnTtsStart += (sender, e) =>
                Console.WriteLine($"Started TTS for text: {e.Text}");

            googleAIService.OnTtsReceiving += (sender, e) =>
            {
                Console.WriteLine($"Progress: {e.ProgressPercentage:F2}%");
                audioProcessor.ProcessAudioChunk(e.AudioData);
            };

            googleAIService.OnTtsFinish += async (sender, e) =>
            {
                Console.WriteLine("TTS streaming completed");
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

        private static async Task TestGoogleSTT(GoogleAIService googleAIService)
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
            Console.WriteLine($"변환된 텍스트: {result}");
        }
        #endregion

    }
}
