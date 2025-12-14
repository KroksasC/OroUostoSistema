using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using System.Security.Claims;

namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController : ControllerBase
    {
        private readonly DataContext _context;

        public LoyaltyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyLoyalty()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var clientId = await _context.Clients
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clientId == 0)
                return Forbid();

            var clients = await _context.Clients
                .Include(c => c.ServiceOrders)
                .ToListAsync();

            var rankedClients = clients
                .Select(c => new RankedClient
                {
                    ClientId = c.Id,
                    Points = CalculatePoints(c)
                })
                .OrderByDescending(x => x.Points)
                .ToList();

            var current = rankedClients.First(x => x.ClientId == clientId);

            int position = rankedClients.FindIndex(x => x.ClientId == clientId) + 1;
            int totalClients = rankedClients.Count;

            string loyaltyLevel = DetermineLoyaltyLevel(position, totalClients);

            return Ok(new
            {
                points = current.Points,
                level = loyaltyLevel,
                benefits = GetBenefitsByLevel(loyaltyLevel)
            });
        }

        private int CalculatePoints(dynamic client)
        {
            int basePoints = client.Points;
            int servicePoints = client.ServiceOrders != null
                ? client.ServiceOrders.Count * 100
                : 0;

            return basePoints + servicePoints;
        }

        private string DetermineLoyaltyLevel(int position, int totalClients)
        {
            double percentile = (double)position / totalClients;

            if (percentile <= 0.2)
                return "Gold";

            if (percentile <= 0.5)
                return "Silver";

            return "Bronze";
        }

        private List<string> GetBenefitsByLevel(string level)
        {
            return level switch
            {
                "Bronze" => new List<string>
                {
                    "Basic customer support"
                },
                "Silver" => new List<string>
                {
                    "Priority boarding",
                    "Extra baggage allowance"
                },
                "Gold" => new List<string>
                {
                    "Priority boarding",
                    "Extra baggage allowance",
                    "Discounts on flights"
                },
                _ => new List<string>()
            };
        }

        private class RankedClient
        {
            public int ClientId { get; set; }
            public int Points { get; set; }
        }
    }
}
