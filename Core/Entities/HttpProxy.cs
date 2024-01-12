using Core.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Entities
{
    public class HttpProxy : BaseModel
    {
        public HttpProxy()
        {
            BlockedBy = new List<CrawlerType>();
        }

        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsProxyRunning { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public List<CrawlerType> BlockedBy { get; set; }
    }
}
