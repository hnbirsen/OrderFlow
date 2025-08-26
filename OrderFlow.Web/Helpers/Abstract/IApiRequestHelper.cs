namespace OrderFlow.Web.Helpers.Abstract
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
