using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

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

        public static ByteArrayContent GetByteArrayContent<T>(T requestModel)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var myContent = JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return byteContent;
        }
    }
}
