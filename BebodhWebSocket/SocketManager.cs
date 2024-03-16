using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BebodhWebSocket
{
    public class SocketManager
    {
        public SocketManager()
        {
            Clients = [];
        }

        public List<Client> Clients { get; set; }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }

        public void RemoveClient(Client client)
        {
            Clients.Remove(client);
        }

        public void RemoveClient(Guid id)
        {
            var clientIndex = Clients.FindIndex(x => x.Id == id);
            if (clientIndex != -1) return;
            Clients.RemoveAt(clientIndex);
        }

        public async Task HandleWebSocketAsync(Client client)
        {
            var buffer = new byte[4 * 1024];
            WebSocketReceiveResult result;

            do
            {
                result = await client.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await BroadcastMessageAsync(json, client.UserId, client.TaskId);
                }
            } while (!result.CloseStatus.HasValue);
        }

        public async Task BroadcastMessageAsync(string message, string? userId = null, string? taskId = null)
        {
            foreach (var client in Clients)
            {
                if (userId != null && !userId.Equals(client.UserId))
                    continue;

                if (client.WebSocket.State == WebSocketState.Open)
                {
                    var messageObject = new
                    {
                        taskId = taskId ?? string.Empty,
                        progress = message
                    };
                    string jsonString = JsonSerializer.Serialize(messageObject);
                    await client.WebSocket.SendAsync(Encoding.UTF8.GetBytes(jsonString), WebSocketMessageType.Text, true, default);
                }
            }
        }
    }
}
