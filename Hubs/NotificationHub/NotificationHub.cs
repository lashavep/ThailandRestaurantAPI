using Microsoft.AspNetCore.SignalR;

namespace RestaurantAPI.Hubs.NotificationHub
{
    public class NotificationHub : Hub                                                      // SignalR ჰაბი შეტყობინებებისთვის
    {
        public override async Task OnConnectedAsync()                                       // ჰაბზე კავშირის დამყარებისას
        {
            var role = Context.User?.FindFirst("role")?.Value;                              // მომხმარებლის როლის მიღება
            if (role == "Admin")                                                            // თუ როლი ადმინისტრატორია
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");               // ადმინისტრატორების ჯგუფში დამატება
            }
            await base.OnConnectedAsync();                                                  // ბაზის მეთოდის გამოძახება
        }
    }
}
