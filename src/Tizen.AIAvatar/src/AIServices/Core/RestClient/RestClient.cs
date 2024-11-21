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
using System.Net.Http;
using System.Threading.Tasks;


namespace Tizen.AIAvatar
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            using var httpRequest = request.CreateRequestMessage(_httpClient.BaseAddress);

            try
            {
                using var httpResponse = await _httpClient.SendAsync(httpRequest);
                var response = new RestResponse
                {
                    StatusCode = httpResponse.StatusCode,
                    IsSuccessful = httpResponse.IsSuccessStatusCode
                };

                if (httpResponse.Content != null)
                {
                    response.RawBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                    response.Content = await httpResponse.Content.ReadAsStringAsync();
                }

                if (!httpResponse.IsSuccessStatusCode)
                {
                    response.ErrorMessage = $"HTTP {(int)httpResponse.StatusCode} - {httpResponse.ReasonPhrase}";
                }

                return response;
            }
            catch (Exception ex)
            {
                return new RestResponse
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
