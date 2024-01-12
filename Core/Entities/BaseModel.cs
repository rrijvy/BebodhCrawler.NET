using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Core.Entities
{
    public class BaseModel
    {
        public ObjectId Id { get; set; }
        public long AddedOn { get; set; }
        public long UpdatedAt { get; set; }
    }
}
