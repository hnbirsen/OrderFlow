using System.Text;
using System.Text.Json;

namespace OrderFlow.Web.Helpers
{
    public class ApiRequestHelper : IApiRequestHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApiRequestHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> SendAsync(
            string url,
            HttpMethod method,
            object? jsonBody = null,
            Dictionary<string, string>? headers = null)
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            // Retrieve access_token from session
            var accessToken = _httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Add custom headers if provided
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var request = new HttpRequestMessage(method, url);

            if (jsonBody != null)
            {
                var json = JsonSerializer.Serialize(jsonBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await client.SendAsync(request);
        }
    }
}