using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseRouting();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
});

var connections = new List<WebSocket>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        connections.Add(ws);
        await HandleWebSocketAsync(ws);
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[4 * 1024];
    WebSocketReceiveResult result;

    do
    {
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastMessageAsync(json);
        }
    } while (!result.CloseStatus.HasValue);

    connections.Remove(webSocket);
    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
}

async Task BroadcastMessageAsync(string message)
{
    foreach (var connection in connections)
    {
        if (connection.State == WebSocketState.Open)
        {
            await connection.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, default);
        }
    }
}

await app.RunAsync();
