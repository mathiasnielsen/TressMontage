using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TressMontage.Client.Core.Http
{
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpRequestExecutor(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null)
            {
                throw new System.Exception("httpClientFactory not valid");
            }

            this.httpClientFactory = httpClientFactory;
        }

        public async Task<TResponse> Put<TResponse>(string url, IEnumerable<KeyValuePair<string, string>> bodyParameter)
            where TResponse : class
        {
            var client = httpClientFactory.CreateHttpClient();
            var content = new FormUrlEncodedContent(bodyParameter);
            var response = await client.PutAsync(url, content);
            return await HandleResponse<TResponse>(response);
        }

        public async Task<TResponse> Post<TContent, TResponse>(string url, TContent bodyParameter)
            where TResponse : class where TContent : class
        {
            var client = httpClientFactory.CreateHttpClient();
            var response = await client.PostAsync(url, CreateJsonContent<TContent>(bodyParameter));
            return await HandleResponse<TResponse>(response);
        }

        public async Task<TResponse> Post<TResponse>(string url, IEnumerable<KeyValuePair<string, string>> bodyParameter)
            where TResponse : class
        {
            var client = httpClientFactory.CreateHttpClient();
            var content = new FormUrlEncodedContent(bodyParameter);
            var response = await client.PostAsync(url, content);
            return await HandleResponse<TResponse>(response);
        }

        public async Task<TResponse> Post<TResponse>(string url, byte[] bodyParameter)
            where TResponse : class
        {
            var client = httpClientFactory.CreateHttpClient();
            var response = await client.PostAsync(url, new ByteArrayContent(bodyParameter));
            return await HandleResponse<TResponse>(response);
        }

        public async Task<TResponse> Get<TResponse>(string url)
            where TResponse : class
        {
            var client = httpClientFactory.CreateHttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            return await HandleResponse<TResponse>(response);
        }

        public async Task<string> GetContent(string url)
        {
            var client = httpClientFactory.CreateHttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            var contentAsString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                return contentAsString;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SecurityException();
            }

            return null;
        }

        private static StringContent CreateJsonContent<TContent>(TContent bodyParameter) where TContent : class
        {
            var content = JsonConvert.SerializeObject(bodyParameter);

            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private static async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            var contentAsString = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<TResponse>(contentAsString);
                }
            }
            catch
            {
                throw;
            }

            return null;
        }
    }
}
