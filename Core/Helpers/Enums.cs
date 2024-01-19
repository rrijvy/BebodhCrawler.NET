using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace Core.Helpers
{
    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CrawlerType
    {
        [BsonIgnoreIfDefault]
        [EnumMember(Value = "None")]
        None,
        [BsonRepresentation(BsonType.String)]
        [EnumMember(Value = "AMAZON")]
        AMAZON,
        [BsonRepresentation(BsonType.String)]
        [EnumMember(Value = "LINKEDIN")]
        LINKEDIN
    }
}
