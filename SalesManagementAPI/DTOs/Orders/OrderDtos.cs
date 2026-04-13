using System.ComponentModel.DataAnnotations;
namespace SalesManagementAPI.DTOs.Orders
{
    // ما يُرسله العميل لإنشاء طلب جديد
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "رقم العميل مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "رقم العميل يجب أن يكون أكبر من صفر")] // يمنع إرسال صفر أو رقم سالب
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "يجب إضافة منتج واحد على الأقل")] // نمنع إنشاء طلب فارغ بدون منتجات
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class CreateOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "الكمية يجب أن تكون بين 1 و 10000")] // نمنع كميات سالبة أو صفرية أو ضخمة جداً
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "الخصم لا يمكن أن يكون سالباً")] 
        public decimal Discount { get; set; } = 0;
    }

    // لتحديث حالة الطلب فقط — بدون الحاجة لإرسال كل بيانات الطلب
    public class UpdateStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }

    // ما يُرجعه API عند قراءة الطلبات — بيانات واضحة للمستخدم
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        public string SalesRepName { get; set; } = string.Empty;
        public int CustomerId { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        // المجموع الفرعي لهذا العنصر: (الكمية × السعر) - الخصم
        public decimal SubTotal { get; set; }
    }
}