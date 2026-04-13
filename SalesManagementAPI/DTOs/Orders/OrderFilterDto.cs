using System.ComponentModel.DataAnnotations;
namespace SalesManagementAPI.DTOs.Orders
{
    // يُستخدم كـ Query Parameters مثل:
    // api/orders?status=Pending&date=2025-01-15
    public class OrderFilterDto
    {
        public DateTime? Date { get; set; }

        // فلترة حسب حالة الطلب
        public OrderStatus? Status { get; set; }

        // فلترة حسب العميل
        public int? CustomerId { get; set; }
    }
}