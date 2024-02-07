using MongoDB.Bson;

namespace Core.Entities
{
    public class BaseModel
    {
        public ObjectId Id { get; set; }
        public long AddedOn { get; set; }
        public long UpdatedOn { get; set; }
    }
}
