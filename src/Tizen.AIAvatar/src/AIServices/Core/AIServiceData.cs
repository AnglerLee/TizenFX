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


namespace Tizen.AIAvatar
{
    public class llmResponseEventArgs : EventArgs
    {
        public int TaskID { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
    }


    public class ttsStreamingEventArgs : EventArgs
    {
        public byte[] AudioData { get; set; }  // 현재 청크의 오디오 데이터
        public int SampleRate { get; set; }
        public string Text { get; set; }
        public string Voice { get; set; }
        public int AudioBytes { get; set; }
        public int TotalBytes { get; set; }
        public int ProcessedBytes { get; set; }
        public double ProgressPercentage { get; set; }
        public string Error { get; set; }        
    }

    public class sttStreamingEventArgs : EventArgs
    {
        public bool Interim { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
    }

    public class ServiceEndpoints
    {
        public string LLMEndpoint { get; set; }
        public string TextToSpeechEndpoint { get; set; }
        public string SpeechToTextEndpoint { get; set; }
    }

    public abstract class AIServiceConfiguration
    {
        public string ApiKey { get; set; }
        public ServiceEndpoints Endpoints { get; set; }
        public Dictionary<string, object> AdditionalSettings { get; set; } = new();
    }

}
