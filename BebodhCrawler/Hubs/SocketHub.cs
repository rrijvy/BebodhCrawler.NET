using Microsoft.AspNetCore.SignalR;

namespace BebodhCrawler.Hubs
{
    public class SocketHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync(Context.ConnectionId);
        }

        public async Task SendMessage()
        {
            await Clients.All.SendAsync("Progress", 50);
        }
    }
}
