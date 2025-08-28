using OrderFlow.Web.Helpers.Abstract;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Web.Helpers.Concrete
{
    public class ApiRequestHelper : IApiRequestHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiRequestHelper> _logger;

        public ApiRequestHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<ApiRequestHelper> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendAsync(
            string url,
            HttpMethod method,
            object? jsonBody = null,
            Dictionary<string, string>? headers = null)
        {
            var client = _httpClientFactory.CreateClient();

            var apiBaseUrl = _configuration["ApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                throw new InvalidOperationException("ApiBaseUrl configuration value is missing or empty.");
            }
            client.BaseAddress = new Uri(apiBaseUrl);

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
                _logger.LogInformation("Sending {Method} request to {Url} with body: {Body}", method, url, json);
            }
            else
            {
                _logger.LogInformation("Sending {Method} request to {Url} with no body.", method, url);
            }

            try
            {
                var response = await client.SendAsync(request);
                _logger.LogInformation("Received response for {Method} {Url}: {StatusCode}", method, url, response.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {Method} request to {Url}", method, url);
                throw;
            }
        }
    }
}