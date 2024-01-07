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
        public static string GetCurrentUnixTime()
        {
            var timeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            return timeStamp;
        }

        public static string GetElapsedTimeInMinute(string lastUsedTimeStamp)
        {
            if (string.IsNullOrEmpty(lastUsedTimeStamp)) return string.Empty;
            DateTimeOffset currentOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(Utility.GetCurrentUnixTime()));
            DateTimeOffset lastUsedOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastUsedTimeStamp));
            var diff = currentOffset - lastUsedOffset;
            return diff.TotalMinutes.ToString();
        }

        public static HttpProxy GetProxy(string proxyAddress)
        {
            return new HttpProxy
            {
                AddedOn = GetCurrentUnixTime(),
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
