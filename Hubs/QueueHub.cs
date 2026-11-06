using Microsoft.AspNetCore.SignalR;

namespace QLedger.Hubs
{
    public class QueueHub : Hub
    {
        public async Task NotifyQueueUpdated()
        {
            await Clients.All.SendAsync("QueueUpdated");
        }
    }
}

