using Core.Helpers;

namespace Core.Models
{
    public class ProxyStatusRequestModel
    {
        public bool? MakeFreez { get; set; }
        public int StatusCode { get; set; }
        public CrawlerType BlockedBy { get; set; }
    }
}
