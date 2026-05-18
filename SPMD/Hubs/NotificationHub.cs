using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SPMD.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
