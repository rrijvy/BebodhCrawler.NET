using Core.Entities;
using System;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class Utility
    {
        public static HttpProxy GetProxy(string proxyAddress)
        {
            return new HttpProxy
            {
                AddedOn = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(),
                IpAddress = proxyAddress,
                IsActive = false,
                IsProxyRunning = false,
            };
        }

        public static async Task<string> GetHtmlAsStringAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                Stream stream = await httpResponse.Content.ReadAsStreamAsync();
                GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                StreamReader reader = new StreamReader(gzipStream);
                var htmlString = await reader.ReadToEndAsync();
                return htmlString;

            }
            else
            {
                var htmlString = await httpResponse.Content.ReadAsStringAsync();
                return htmlString;
            }
        }
    }
}
