namespace OrderFlow.Web.Helpers
{
    public interface IApiRequestHelper
    {
        Task<HttpResponseMessage> SendAsync(
            string url,
            HttpMethod method,
            object? jsonBody = null,
            Dictionary<string, string>? headers = null);
    }
}
