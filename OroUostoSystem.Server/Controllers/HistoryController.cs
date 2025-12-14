using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using System.Security.Claims;

namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly DataContext _context;

        public HistoryController(DataContext context)
        {
            _context = context;
        }

        // GET: api/history/services
        // ClientServiceHistory.jsx
        [Authorize]
        [HttpGet("services")]
        public async Task<IActionResult> GetMyServiceHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
                return Forbid();

            var serviceOrders = await _context.ServiceOrders
                .Include(o => o.Service)
                .Where(o => o.ClientId == client.Id)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    serviceName = o.Service.Title,
                    category = o.Service.Category,
                    orderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    quantity = o.Quantity,
                    totalPrice = o.TotalPrice
                })
                .ToListAsync();

            if (!serviceOrders.Any())
            {
                return Ok(new
                {
                    message = "You don't have any service order history yet."
                });
            }

            return Ok(serviceOrders);
        }
    }
}
