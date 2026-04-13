using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagementAPI.Services;
using SalesManagementAPI.DTOs.Orders;
using System.Security.Claims;

namespace SalesManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilterDto filter)
        {
            var result = await _service.GetOrdersAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var result = await _service.CreateOrderAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderDto dto)
        {
            var success = await _service.UpdateOrderAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPatch("status/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var success = await _service.UpdateOrderStatusAsync(id, dto.Status);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteOrderAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}