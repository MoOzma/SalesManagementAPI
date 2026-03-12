
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty; // مثال: ORD-2025-001
    public int UserId { get; set; }           // Foreign Key
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending = 0,    // قيد الانتظار
    Confirmed = 1,  // مؤكد
    Shipped = 2,    // تم الشحن
    Delivered = 3,  // تم التسليم
    Cancelled = 4   // ملغي
}
