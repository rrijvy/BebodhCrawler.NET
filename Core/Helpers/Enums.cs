using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Core.Helpers
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrawlerType
    {
        AMAZON,
        LINKEDIN
    }
}
