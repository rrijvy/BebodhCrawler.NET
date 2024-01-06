namespace Core.Entities
{
    public class Restaurant : BaseModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string OpenAt { get; set; }
        public string CloseAt { get; set; }
    }
}
