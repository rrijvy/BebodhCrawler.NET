using Core.Helpers;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class ProxyRequestModel
    {
        public int Count { get; set; }

        public List<CrawlerType>? CrawlerTypes { get; set; }
    }
}
