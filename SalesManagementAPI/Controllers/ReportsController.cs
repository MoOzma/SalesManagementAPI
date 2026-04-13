using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesManagementAPI.Data.Repositories;

namespace SalesManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : ControllerBase
    {
        private readonly IOrderRepository _repo;

        public ReportsController(IOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> Daily([FromQuery] DateTime date)
        {
            var report = await _repo.GetDailySalesReportAsync(date);
            return Ok(report);
        }
    }
}