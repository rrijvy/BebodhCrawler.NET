using System.Net;
using System.Net.Http;

namespace Core.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient GetHttpClient(string proxy)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"http://{proxy}"),
                UseProxy = true
            };
            var httpClient = new HttpClient(handler);
            var browserAgent = BrowserUserAgentHelper.GetRandomBrowserAgent();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(browserAgent);
            return httpClient;
        }
    }
}
