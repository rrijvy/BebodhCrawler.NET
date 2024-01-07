using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

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
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            return httpClient;
        }
    }
}
