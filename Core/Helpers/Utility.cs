using Core.Entities;
using System;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Helpers
{
    public static class Utility
    {
        public static string GetCurrentUnixTimeAsString()
        {
            var timeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            return timeStamp;
        }

        public static long GetCurrentUnixTime()
        {
            var timeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            return timeStamp;
        }

        public static long GetMinimumUnixTime()
        {
            var timeStamp = new DateTimeOffset(DateTime.MinValue).ToUnixTimeSeconds();
            return timeStamp;
        }


        public static double GetElapsedTimeInSecond(string lastUsedTimeStamp)
        {
            if (string.IsNullOrEmpty(lastUsedTimeStamp)) return 0;
            DateTimeOffset currentOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(Utility.GetCurrentUnixTimeAsString()));
            DateTimeOffset lastUsedOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastUsedTimeStamp));
            var diff = currentOffset - lastUsedOffset;
            return diff.TotalSeconds;
        }

        public static double GetElapsedTimeInSecond(long lastUsedTimeStamp)
        {
            if (lastUsedTimeStamp <= GetMinimumUnixTime()) return 0;
            DateTimeOffset currentOffset = DateTimeOffset.FromUnixTimeSeconds(Utility.GetCurrentUnixTime());
            DateTimeOffset lastUsedOffset = DateTimeOffset.FromUnixTimeSeconds(lastUsedTimeStamp);
            var diff = currentOffset - lastUsedOffset;
            return diff.TotalSeconds;
        }

        public static HttpProxy GetProxy(string proxyAddress)
        {
            return new HttpProxy
            {
                Id = proxyAddress,
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

        public static string? GetNodeInnerText(HtmlNode parentNode, string xpath, string attribute = null)
        {
            try
            {
                var node = parentNode.SelectNodes(xpath)?[0];
                return node?.InnerText.Trim();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static decimal GetNodeInnerTextAsDecimal(HtmlNode parentNode, string xpath)
        {
            try
            {
                var node = parentNode.SelectNodes(xpath)?[0];
                if (decimal.TryParse(node?.InnerText.Trim(), out decimal value))
                    return value;
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double GetNodeInnerTextAsDouble(HtmlNode parentNode, string xpath)
        {
            try
            {
                var node = parentNode.SelectNodes(xpath)?[0];
                if (double.TryParse(node?.InnerText.Trim(), out double value))
                    return value;
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string? GetNodeAttributeValue(HtmlNode parentNode, string xpath, string attributeName)
        {
            try
            {
                var node = parentNode.SelectNodes(xpath)?[0];
                return node?.GetAttributeValue(attributeName, "").Trim();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GetPropertyName<T>(Expression<Func<T, string>> expression)
        {
            if (expression.Body is not MemberExpression memberExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }

            if (memberExpression != null)
            {
                PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;

                if (propertyInfo != null)
                {
                    return propertyInfo.Name;
                }
            }

            throw new ArgumentException("Expression is not a valid property expression");
        }
    }
}
