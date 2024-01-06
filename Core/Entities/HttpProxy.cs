namespace Core.Entities
{
    public class HttpProxy : BaseModel
    {
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsProxyRunning { get; set; }
    }
}
