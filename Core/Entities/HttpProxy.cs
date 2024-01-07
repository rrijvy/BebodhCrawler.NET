using Core.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Core.Entities
{
    public class HttpProxy : BaseModel
    {
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsProxyRunning { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<CrawlerType> BlockedBy { get; set; }
    }
}
