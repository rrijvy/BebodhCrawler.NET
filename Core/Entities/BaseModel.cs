using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Core.Entities
{
    public class BaseModel
    {
        public ObjectId Id { get; set; }
        public string AddedOn { get; set; }
        public string UpdatedAt { get; set; }
    }
}
