using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Services;

namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly DataContext _context;

        public ServiceController(EmailService emailService, DataContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        [HttpPost("OrderService")]
        public async Task<IActionResult> OrderService([FromBody] ServiceOrderDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == dto.UserId);
            if (client == null)
                return BadRequest("Client not found.");

            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return BadRequest("Service not found.");

            var order = new ServiceOrder
            {
                OrderDate = dto.OrderDate,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                ClientId = client.Id,
                ServiceId = dto.ServiceId,
                Client = client,
                Service = service
            };

            try
            {
                _context.ServiceOrders.Add(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return BadRequest("Failed to save order to database.");
            }

            try
            {
            await _emailService.SendServiceOrderConfirmationAsync(
                    dto.Email,
                    dto.ServiceName,
                    dto.TotalPrice
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
                return BadRequest("Order saved, but failed to send email.");
            }

            return Ok("Order created and confirmation email sent.");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO dto)
        {

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return BadRequest("Employee not found.");

            var service = new Service
            {
                Title = dto.Title,
                Price = dto.Price,
                Category = dto.Category,
                Description = dto.Description,
                EmployeeId = dto.EmployeeId,
                Employee = employee
            };

            try
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving service: " + ex.Message);
                return BadRequest("Failed to create service.");
            }

            return Ok("Created successfully!");
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceDTO dto)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound("Service not found.");

            service.Title = dto.Title;
            service.Price = dto.Price;
            service.Category = dto.Category;
            service.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating service: " + ex.Message);
                return BadRequest("Failed to update service.");
            }

            return Ok("Updated successfuly!");
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services
                .Include(s => s.ServiceOrders)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound("Service not found.");

            try
            {
                // 1️⃣ Delete all related service orders
                if (service.ServiceOrders.Any())
                {
                    _context.ServiceOrders.RemoveRange(service.ServiceOrders);
                }

                // 2️⃣ Delete service
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting service: " + ex.Message);
                return BadRequest("Failed to delete service.");
            }

            return Ok("Service and all related service orders deleted successfully.");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _context.Services
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.Price,
                    s.Category,
                    s.Description
                })
                .ToListAsync();

            return Ok(services);
        }
    }
}
