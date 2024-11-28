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
using System.Net;
using System.Threading.Tasks;

namespace Tizen.AIAvatar
{

    /// <summary>
    /// Represents the response from a REST API call.
    /// </summary>
    public class RestResponse
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// The HTTP status code returned by the server.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The content of the response as a string.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The raw bytes of the response.
        /// </summary>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// The error message if the request failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Interface for making REST API calls.
    /// </summary>
    public interface IRestClient : IDisposable
    {
        /// <summary>
        /// Executes a REST request asynchronously.
        /// </summary>
        /// <param name="request">The REST request to execute.</param>
        /// <returns>A task representing the asynchronous operation, which returns the REST response.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the request parameter is null.</exception>
        Task<RestResponse> ExecuteAsync(RestRequest request);
    }
}