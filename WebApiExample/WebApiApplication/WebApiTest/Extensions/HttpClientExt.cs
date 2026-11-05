using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApiTest.Extensions;

internal static class HttpClientExt
{
    extension (HttpClient httpClient)
    {
        public async Task<string?> PostContentAsync<TResponse, TRequest>(string queryStr, TRequest @object, CancellationToken token) 
            where TRequest : notnull 
        {
            var response = await httpClient.PostAsJsonAsync(queryStr, @object, token);
            var content = await response.Content.ReadAsStringAsync(token);
            if (response.IsSuccessStatusCode)
            {
                return content;
            }
            else
            {
                throw new Exception(content);
            }
        }

        public async Task<TResponse?> PostResponseAsync<TResponse, TRequest>(string queryStr, TRequest request, CancellationToken token)
        {
            var response = await httpClient.PostAsJsonAsync(queryStr, request, token);
            var content = await response.Content.ReadAsStringAsync(token);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(content);
            }
            else
            {
                throw new Exception(content);
            }
        }

        public async Task<TResponse?> GetResponseAsync<TResponse>(string queryStr, CancellationToken token)
        {
            var response = await httpClient.GetAsync(queryStr, token);
            var content = await response.Content.ReadAsStringAsync(token);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(content);
            }
            else
            {
                throw new Exception(content);
            }
        }

        public HttpClient AuthorizationBearer(string securityToken)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", securityToken);
            return httpClient;
        }
    }
}
