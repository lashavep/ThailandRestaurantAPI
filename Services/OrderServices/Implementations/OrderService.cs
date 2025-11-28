using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.OrderDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.OrderRepos.Interfaces;
using RestaurantAPI.Services.EmailService.Interfaces;
using RestaurantAPI.Services.OrderServices.Interfaces;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _emailService;

    public OrderService(IOrderRepository orderRepo, ApplicationDbContext db, IEmailService emailService)
    {
        _orderRepo = orderRepo;
        _db = db;
        _emailService = emailService;
    }

    public async Task<Order> PlaceOrderAsync(CreateOrderDTO dto, int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new Exception("User not found");

        var order = new Order
        {
            UserId = userId,
            ItemsJson = JsonConvert.SerializeObject(dto.Items ?? new List<OrderItemDTO>()),
            Total = dto.Total,
            Address = user.Address,
            Date = DateTime.Now,
            Status = "Pending"
        };

        return await _orderRepo.AddAsync(order);
    }

    public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserAsync(int userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);

        return orders.Select(o => new OrderResponseDTO
        {
            Id = o.Id,
            UserName = o.User?.FirstName ?? "",
            Address = o.Address,
            Total = o.Total,
            Date = o.Date,
            Items = string.IsNullOrEmpty(o.ItemsJson)
                ? new List<OrderItemDTO>()
                : JsonConvert.DeserializeObject<List<OrderItemDTO>>(o.ItemsJson)!
        });
    }

    public async Task<(bool Success, string Message, Order? Order)> AcceptOrderAsync(int id)
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return (false, "Order not found", null);

        order.Status = "Complete";
        await _db.SaveChangesAsync();

        string deliveryMessage = DateTime.Now.Hour < 12
            ? "Your order will be delivered today."
            : "Your order will be delivered tomorrow.";

        await _emailService.SendEmailAsync(order.User.Email, "Order Status Update",
            $"Hello {order.User.FirstName},\n\nYour order #{order.Id} has been accepted.\n{deliveryMessage}");

        return (true, "Order accepted, email sent", order);
    }

    public async Task<(bool Success, string Message, Order? Order)> RejectOrderAsync(int id)
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return (false, "Order not found", null);

        order.Status = "Rejected";
        await _db.SaveChangesAsync();

        await _emailService.SendEmailAsync(order.User.Email, "Order Status Update",
            $"გამარჯობა {order.User.FirstName},\n\nთქვენი შეკვეთა #{order.Id} უარყოფილია.");

        return (true, "Order rejected, email sent", order);
    }

    public async Task<object?> GetOrderDetailsAsync(int id)
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return null;

        var products = string.IsNullOrEmpty(order.ItemsJson)
            ? new List<OrderItemDTO>()
            : JsonConvert.DeserializeObject<List<OrderItemDTO>>(order.ItemsJson)!;

        return new
        {
            order.Id,
            User = new { order.User.Id, order.User.FirstName, order.User.Email },
            Products = products,
            order.Total,
            order.Address,
            order.Date,
            order.Status
        };
    }

    public async Task<object> GetAllOrdersAsync(int page, int pageSize)
    {
        var query = _db.Orders.Include(o => o.User).OrderByDescending(o => o.Date);
        var totalCount = await query.CountAsync();
        var orders = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = orders.Select(o => new
        {
            o.Id,
            User = new { o.User.Id, o.User.FirstName, o.User.Email },
            o.Total,
            o.Address,
            o.Date,
            o.Status,
            Products = string.IsNullOrEmpty(o.ItemsJson)
                ? new List<OrderItemDTO>()
                : JsonConvert.DeserializeObject<List<OrderItemDTO>>(o.ItemsJson)!
        });

        return new { totalCount, page, pageSize, orders = result };
    }
}
