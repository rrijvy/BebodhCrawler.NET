using BebodhWebSocket;
using System.Net;

var socketManager = new SocketManager();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseRouting();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
});


app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var queryParams = context.Request.Query;

        var userId = queryParams["userid"];
        var taskId = queryParams["taskid"];

        if (string.IsNullOrEmpty(userId.ToString()))
            return;

        using var websocket = await context.WebSockets.AcceptWebSocketAsync();

        var client = new Client
        {
            Id = new Guid(),
            UserId = userId.ToString(),
            TaskId = taskId.ToString(),
            WebSocket = websocket,
        };

        socketManager.AddClient(client);

        await socketManager.HandleWebSocketAsync(client);
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

await app.RunAsync();
