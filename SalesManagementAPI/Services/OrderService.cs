// Services/OrderService.cs
using SalesManagementAPI.Data.Repositories;
using SalesManagementAPI.DTOs.Orders;

namespace SalesManagementAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(OrderFilterDto filter);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, int userId);
        Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status);
        Task<bool> DeleteOrderAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IRepository<Product> _productRepo;

        public OrderService(IOrderRepository orderRepo, IRepository<Product> productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        // إنشاء طلب جديد
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, int userId)
        {
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                CustomerId = dto.CustomerId,
                Status = OrderStatus.Pending
            };

            decimal total = 0;

            foreach (var itemDto in dto.Items)
            {
                // التحقق من وجود المنتج
                var product = await _productRepo.GetByIdAsync(itemDto.ProductId)
                    ?? throw new KeyNotFoundException($"المنتج رقم {itemDto.ProductId} غير موجود");

                // التحقق من الكمية المتاحة
                if (product.StockQuantity < itemDto.Quantity)
                    throw new InvalidOperationException(
                        $"الكمية المتاحة من {product.Name} هي {product.StockQuantity} فقط");

                var item = new OrderItem
                {
                    ProductId = product.Id,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity,
                    Discount = itemDto.Discount
                };

                order.Items.Add(item);
                total += item.SubTotal;

                // تقليل المخزون
                product.StockQuantity -= itemDto.Quantity;
                _productRepo.Update(product);
            }

            order.TotalAmount = total;

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            return await MapToResponseDto(order);
        }

        // تحديث حالة الطلب
        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("لا يمكن تعديل طلب ملغي");

            order.Status = newStatus;
            _orderRepo.Update(order);

            return await _orderRepo.SaveChangesAsync();
        }

        // توليد رقم طلب فريد
        private static string GenerateOrderNumber()
        {
            var year = DateTime.UtcNow.Year;
            var unique = Guid.NewGuid().ToString("N")[..8].ToUpper();
            return $"ORD-{year}-{unique}";
        }

        private static OrderResponseDto MapToResponseDto(Order order) => new()
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SalesRepName = order.User?.FullName ?? "",
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductName = i.Product?.Name ?? "",
                SKU = i.Product?.SKU ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                SubTotal = i.SubTotal
            }).ToList()
        };

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(OrderFilterDto filter)
        {
            var orders = await _orderRepo.FindAsync(o =>
                (!filter.Date.HasValue || o.OrderDate.Date == filter.Date.Value.Date) &&
                (!filter.Status.HasValue || o.Status == filter.Status.Value) &&
                (!filter.CustomerId.HasValue || o.CustomerId == filter.CustomerId.Value));

            return orders.Select(MapToResponseDto);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetOrderWithItemsAsync(id);
            return order == null ? null : MapToResponseDto(order);
        }

        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            if (dto.Notes != null)
                order.Notes = dto.Notes;

            _orderRepo.Update(order);
            return await _orderRepo.SaveChangesAsync();
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            _orderRepo.Delete(order);
            return await _orderRepo.SaveChangesAsync();
        }
    }
}