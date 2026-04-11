// DTOs/Reports/ReportDtos.cs
namespace SalesManagementAPI.DTOs.Reports
{
    // تقرير المبيعات اليومي الكامل — يُرجعه الـ ReportsController للمستخدم
    public class DailySalesReport
    {
        public DateTime Date { get; set; } // تاريخ التقرير

        // عدد الطلبات الكلي في هذا اليوم
        public int TotalOrders { get; set; }

        // مجموع قيمة كل الطلبات في هذا اليوم
        public decimal TotalRevenue { get; set; }

        // متوسط قيمة الطلب الواحد (إجمالي الإيرادات / عدد الطلبات)
        public decimal AverageOrderValue { get; set; }

        // إحصائيات حسب حالة الطلب
        public int CompletedOrders { get; set; } // الطلبات التي وصلت (Delivered)
        public int PendingOrders { get; set; }   // الطلبات قيد الانتظار (Pending)
        public int CancelledOrders { get; set; } // الطلبات الملغاة (Cancelled)

        // قائمة بأعلى 5 منتجات مبيعاً في هذا اليوم
        public List<ProductSalesSummary> TopProducts { get; set; } = new();
    }

    // ملخص مبيعات منتج واحد — يُستخدم كجزء من تقرير DailySalesReport
    public class ProductSalesSummary
    {
        // اسم المنتج
        public string ProductName { get; set; } = string.Empty;

        // مجموع الكميات المباعة من هذا المنتج خلال اليوم
        public int TotalSold { get; set; }

        // مجموع الإيرادات الناتجة عن هذا المنتج فقط
        public decimal TotalRevenue { get; set; }
    }

    // تقرير فترة زمنية مخصصة — من تاريخ إلى تاريخ
    public class DateRangeReport
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // تفصيل المبيعات يوماً بيوم داخل هذه الفترة
        public List<DailySalesSummary> DailyBreakdown { get; set; } = new();
    }

    // ملخص بسيط ليوم واحد — يُستخدم داخل تقرير الفترة الزمنية (DateRangeReport)
    public class DailySalesSummary
    {
        public DateTime Date { get; set; }
        public int OrdersCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
///,;l,;l,l