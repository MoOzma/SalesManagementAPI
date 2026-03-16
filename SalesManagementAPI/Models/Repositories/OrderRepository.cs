using Microsoft.EntityFrameworkCore;
using SalesManagementAPI.DTOs.Reports;

namespace SalesManagementAPI.Data.Repositories
{
    // Interface: مخصص يرث من Generic Interface ويضيف عمليات خاصة بالطلبات فقط
    public interface IOrderRepository : IRepository<Order>
    {
        // عملية خاصة: جلب طلب مع كل عناصره ومنتجاتها (لا يمكن تعميمها في Generic)
        Task<Order?> GetOrderWithItemsAsync(int orderId);

        // تقرير يومي مخصص للمبيعات
        Task<DailySalesReport> GetDailySalesReportAsync(DateTime date);

        // بحث عن الطلبات بتاريخ محدد
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
    }

    // التنفيذ الفعلي: يرث من GenericRepository لنحصل على العمليات الأساسية مجاناً
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        // نمرر الـ context للـ GenericRepository عبر الـ base
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                // Include: Eager Loading يجلب البيانات المرتبطة (مثل عناصر الطلب) في استعلام واحد
                // بدونها سيولد EF Core استعلامات منفصلة لكل عنصر (N+1 Problem) مما يقلل الأداء
                .Include(o => o.Items)
                    // ThenInclude: يجلب المنتج المرتبط بكل عنصر داخل قائمة العناصر
                    .ThenInclude(i => i.Product)
                .Include(o => o.User)
                // FirstOrDefaultAsync: يرجع أول نتيجة أو null ولا يرمي Exception إذا لم يجد شيئاً
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _context.Orders
                // مقارنة التاريخ فقط (بدون الوقت) لضمان دقة البحث اليومي
                .Where(o => o.OrderDate.Date == date.Date)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate) // ترتيب من الأحدث للأقدم
                .AsNoTracking() // أسرع للقراءة فقط لأن EF لن يتابع التغييرات
                .ToListAsync();
        }

        public async Task<DailySalesReport> GetDailySalesReportAsync(DateTime date)
        {
            var orders = (await GetOrdersByDateAsync(date)).ToList();

            return new DailySalesReport
            {
                Date = date,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),

                // حساب عدد الطلبات بناءً على حالتها باستخدام Lambda
                CompletedOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),

                // حساب متوسط قيمة الطلب مع تفادي خطأ القسمة على صفر (DivideByZero)
                AverageOrderValue = orders.Any()
                    ? orders.Average(o => o.TotalAmount)
                    : 0,//MKK

                // تقرير بأعلى 5 منتجات مبيعاً خلال اليوم
                TopProducts = orders
                    .SelectMany(o => o.Items) // فرد كل عناصر الطلبات في قائمة واحدة
                    .GroupBy(i => i.Product.Name) // تجميعها حسب اسم المنتج
                    .OrderByDescending(g => g.Sum(i => i.Quantity)) // ترتيب حسب إجمالي الكمية المباعة
                    .Take(5) // أخذ أعلى 5 منتجات فقط
                    .Select(g => new ProductSalesSummary
                    {
                        ProductName = g.Key,
                        TotalSold = g.Sum(i => i.Quantity),
                   //     TotalRevenue = g.Sum(i => i.SubTotal)
                    })
                    .ToList()
            };
        }
    }
}