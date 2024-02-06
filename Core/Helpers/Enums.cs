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

    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RecurrenceType
    {
        Daily,
        Weekly,
        Monthly
    }

    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WeekDay
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MonthlyRecurrenceWeek
    {
        NotDefined,
        First,
        Second,
        Third,
        Fourth,
        Last
    }

    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MonthlySelectionType
    {
        NotDefined,
        OnSpecificDay,
        OnRecurrenceWeekDay
    }
}
