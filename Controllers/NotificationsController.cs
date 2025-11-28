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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetNotifications([FromServices] ApplicationDbContext db)
        {
            var nots = await db.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            return Ok(nots);
        }
    }
}
