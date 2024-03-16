using System.Net.WebSockets;

namespace BebodhWebSocket
{
    public class Client
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string TaskId { get; set; }
        public WebSocket WebSocket { get; set; }
    }
}
