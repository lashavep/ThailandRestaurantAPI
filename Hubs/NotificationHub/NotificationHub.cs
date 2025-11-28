using Microsoft.AspNetCore.SignalR;
using RestaurantAPI.Models;

namespace RestaurantAPI.Hubs.NotificationHub
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var role = Context.User?.FindFirst("role")?.Value;
            if (role == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            await base.OnConnectedAsync();
        }
    }
}
