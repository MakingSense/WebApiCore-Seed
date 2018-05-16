using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Seed.Infrastructure.RestClient
{
    /// <summary>
    /// <see cref="IRestClient"/> implementation relying on <see cref="HttpClient"/> and <see cref="Newtonsoft.Json.JsonConvert"/> to easily perform REST request and parse JSON responses
    /// </summary>
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        /// <param name="baseUrl"> Base URL for the REST api to be consumed by this instance </param>
        /// <param name="httpClient"> <see cref="HttpClient"/> used to perform requests </param>
        public RestClient(string baseUrl, HttpClient httpClient)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _baseUri))
                throw new ArgumentException(nameof(baseUrl), $"Invalid base URL for {nameof(RestClient)}");

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<RestResponse<TResource, TError>> PostAsync<TResource, TError>(string relativeUrl, object body)
        {
            var httpContent = body == null ? null : new StringContent(JsonConvert.SerializeObject(body, Formatting.None), Encoding.UTF8, "application/json");
            return await ExecuteRequest<TResource, TError>(HttpMethod.Post, relativeUrl, httpContent);
        }

        /// <inheritdoc />
        public async Task<RestResponse<TResource, TError>> GetAsync<TResource, TError>(string relativeUrl)
        {
            return await ExecuteRequest<TResource, TError>(HttpMethod.Get, relativeUrl, null);
        }

        private async Task<RestResponse<TResource, TError>> ExecuteRequest<TResource, TError>(HttpMethod method, string relativeUrl, HttpContent content)
        {
            var request = PrepareRequest(method, relativeUrl, content);
            // request.Headers.Add
            var httpResponse = await _httpClient.SendAsync(request);
            return await ParseResponse<TResource, TError>(httpResponse);
        }

        private async Task<RestResponse<TResource, TError>> ParseResponse<TResource, TError>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && TryParseJson(responseContent, out TResource content))
            {
                return new RestResponse<TResource, TError>(response.StatusCode, content);
            }

            TryParseJson(responseContent, out TError error);
            return new RestResponse<TResource, TError>(response.StatusCode, error);
        }

        private HttpRequestMessage PrepareRequest(HttpMethod method, string relativeUrl, HttpContent requestBody)
        {
            if (string.IsNullOrEmpty(relativeUrl) || !Uri.TryCreate(_baseUri, relativeUrl, out Uri absoluteUrl))
                throw new ArgumentException($"Invalid relative url: {relativeUrl}", nameof(relativeUrl));

            return new HttpRequestMessage
            {
                RequestUri = absoluteUrl,
                Method = method,
                Content = requestBody
            };
        }

        private bool TryParseJson<T>(string json, out T content)
        {
            try
            {
                content = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                content = default(T);
            }

            return !Equals(content, default(T));
        }
    }
}
