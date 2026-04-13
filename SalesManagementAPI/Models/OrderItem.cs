public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // أضف هذه الأسطر الثلاثة يدوياً
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int Quantity { get; set; }

    // هذه الخاصية (SubTotal) تُحسب تلقائياً ولا تحتاج لتخزينها في قاعدة البيانات
    public decimal SubTotal => (UnitPrice * Quantity) - Discount;

    public Order Order { get; set; }


    public Product Product { get; set; }


    public decimal Price { get; set; }
}