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

using System.Text.Json;
using System.Text;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tizen.AIAvatar
{

    public enum Method
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }
    public class RestRequest
    {
        public string Resource { get; }
        public Method Method { get; }
        private readonly Dictionary<string, string> _headers;
        private object _body;
        private string _jsonStringBody;

        public RestRequest(Method method)
        {
            Resource = string.Empty;
            Method = method;
            _headers = new Dictionary<string, string>();
        }

        public RestRequest(string resource, Method method)
        {
            Resource = resource;
            Method = method;
            _headers = new Dictionary<string, string>();
        }

        public RestRequest AddHeader(string name, string value)
        {
            _headers[name] = value;
            return this;
        }

        public RestRequest AddJsonBody(object body)
        {
            _body = body;
            _jsonStringBody = null; // Clear json string if object body is set
            if (!_headers.ContainsKey("Content-Type"))
            {
                _headers["Content-Type"] = "application/json";
            }
            return this;
        }

        public RestRequest AddJsonStringBody(string jsonString)
        {
            _jsonStringBody = jsonString;
            _body = null; // Clear object body if json string is set
            if (!_headers.ContainsKey("Content-Type"))
            {
                _headers["Content-Type"] = "application/json";
            }
            return this;
        }

        internal HttpRequestMessage CreateRequestMessage(Uri baseAddress)
        {
            // Resource가 비어있으면 baseAddress만 사용
            var requestUri = string.IsNullOrEmpty(Resource)
                ? baseAddress
                : new Uri(baseAddress, Resource);

            var request = new HttpRequestMessage(GetHttpMethod(), requestUri);

            foreach (var header in _headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (_jsonStringBody != null)
            {
                request.Content = new StringContent(_jsonStringBody, Encoding.UTF8, "application/json");
            }
            else if (_body != null)
            {
                var jsonBody = JsonSerializer.Serialize(_body, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            return request;
        }

        private HttpMethod GetHttpMethod()
        {
            return Method switch
            {
                Method.Get => HttpMethod.Get,
                Method.Post => HttpMethod.Post,
                Method.Put => HttpMethod.Put,
                Method.Delete => HttpMethod.Delete,
                Method.Patch => HttpMethod.Patch,
                _ => throw new ArgumentException($"Unsupported HTTP method: {Method}")
            };
        }
    }
}
