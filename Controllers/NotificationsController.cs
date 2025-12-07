using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public NotificationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetAllNotifications")]
        public async Task<IActionResult> GetNotifications([FromServices] ApplicationDbContext db) // მეთოდი ყველა წაკითხული შეტყობინების მისაღებად
        {
            var nots = await db.Notifications                                                     // მიღება მონაცემთა ბაზიდან
                .Where(n => !n.IsRead)                                                            // ფილტრაცია წაკითხული შეტყობინებების მიხედვით
                .ToListAsync();                                                                   //მიღება სიის სახით

            return Ok(nots);                                                                      // HTTP 200 პასუხის დაბრუნება შეტყობინებების სიით
        }
    }
}
