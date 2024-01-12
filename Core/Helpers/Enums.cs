using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Helpers
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrawlerType
    {
        [BsonRepresentation(BsonType.String)]
        AMAZON,
        [BsonRepresentation(BsonType.String)]
        LINKEDIN
    }
}
