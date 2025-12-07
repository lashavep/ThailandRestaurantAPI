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

    public async Task<Order> PlaceOrderAsync(CreateOrderDTO dto, int userId)                            // შეკვეთის განთავსება
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);                            // მომხმარებლის მიღება ID-ის მიხედვით
        if (user == null) throw new Exception("User not found");

        var order = new Order                                                                           // შეკვეთის ახალი ობიექტის შექმნა
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

    public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserAsync(int userId)                   //  შეკვეთების მიღება მომხმარებლების მიხედვით
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);                                         // მომხმარებლის მიერ განთავსებული შეკვეთების მიღება

        return orders.Select(o => new OrderResponseDTO                                                  // DTO-ში გადამყვანი მეთოდის გამოყენება
        {
            Id = o.Id,
            UserName = o.User?.FirstName ?? "",
            Address = o.Address,
            Total = o.Total,
            Date = o.Date,
            Items = string.IsNullOrEmpty(o.ItemsJson)
                ? new List<OrderItemDTO>()
                : JsonConvert.DeserializeObject<List<OrderItemDTO>>(o.ItemsJson)!                       // შეკვეთის ნივთების დესერალიზაცია JSON-დან
        });
    }

    public async Task<(bool Success, string Message, Order? Order)> AcceptOrderAsync(int id)            // შეკვეთის დადასტურება
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);         // შეკვეთის მიღება იუზერის ID-ის მიხედვით
        if (order == null) return (false, "Order not found", null);

        order.Status = "Complete";                                                                      // სტატუსის განახლება "Complete"-ზე
        await _db.SaveChangesAsync();

        string deliveryMessage = DateTime.Now.Hour < 12
            ? "Your order will be delivered today."
            : "Your order will be delivered tomorrow.";

        await _emailService.SendEmailAsync(order.User.Email, "Order Status Update",
            $"Hello {order.User.FirstName},\n\nYour order #{order.Id} has been accepted.\n{deliveryMessage}");

        return (true, "Order accepted, email sent", order);
    }

    public async Task<(bool Success, string Message, Order? Order)> RejectOrderAsync(int id)             // შეკვეთის უარყოფა
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);         // შეკვეთის მიღება იუზერის ID-ის მიხედვით
        if (order == null) return (false, "Order not found", null);

        order.Status = "Rejected";
        await _db.SaveChangesAsync();

        await _emailService.SendEmailAsync(order.User.Email, "Order Status Update",
            $"გამარჯობა {order.User.FirstName},\n\nთქვენი შეკვეთა #{order.Id} უარყოფილია.");

        return (true, "Order rejected, email sent", order);
    }

    public async Task<object?> GetOrderDetailsAsync(int id)                                             // შეკვეთის დეტალების მიღება იუზერის ID-ის მიხედვით
    {
        var order = await _db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);         
        if (order == null) return null;

        var products = string.IsNullOrEmpty(order.ItemsJson)
            ? new List<OrderItemDTO>()
            : JsonConvert.DeserializeObject<List<OrderItemDTO>>(order.ItemsJson)!; 

        return new                                                                                      // შეკვეთის დეტალების ობიექტის შექმნა
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

    public async Task<object> GetOrdersByStatusAsync(string status, int page, int pageSize)             // სტატუსის მიხედვით შეკვეთების მიღება, გვერდების მითითებით
    {
        var query = _db.Orders.Include(o => o.User).AsQueryable();                                      // შეკვეთების მიღება იუზერის მონაცემებით

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);                                               // სტატუსის მიხედვით ფილტრაცია

        var totalCount = await query.CountAsync();                                                      // შეკვეთების საერთო რაოდენობის მიღება
        var orders = await query
            .OrderByDescending(o => o.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = orders.Select(o => new                                                             // DTO-ში გადამყვანი მეთოდის გამოყენება
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

        return new { totalCount, page, pageSize, orders = result };                                     // შეკვეთების დეტალების ობიექტის შექმნა გვერდებით
    }

}
