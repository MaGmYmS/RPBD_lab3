using Microsoft.AspNetCore.SignalR;

namespace AreasDataBase.Controllers
{
    public class UpdateHub : Hub
    {
        public async Task SendUpdateNotification()
        {
            await Clients.All.SendAsync("UpdateReceived");
        }
    }
}
