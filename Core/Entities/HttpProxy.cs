using Core.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Entities
{
    public class HttpProxy
    {
        public HttpProxy()
        {
            BlockedBy = new List<CrawlerType>();
        }

        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsProxyRunning { get; set; }
        public bool IsFreezed { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public List<CrawlerType> BlockedBy { get; set; }
        public long AddedOn { get; set; }
        public long UpdatedOn { get; set; }
    }
}
