// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تعريف العلاقات
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        // تحديد دقة الأرقام العشرية
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        // إنشاء Index على البريد الإلكتروني
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        // إضافة بيانات تجريبية (Seed Data)
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "لابتوب", SKU = "LAP-001", Price = 4500, StockQuantity = 50 },
            new Product { Id = 2, Name = "هاتف ذكي", SKU = "PHN-001", Price = 2200, StockQuantity = 100 }
        );
    }
}
