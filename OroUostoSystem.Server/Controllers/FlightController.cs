using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;

namespace OroUostoSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<FlightController> _logger;

        public FlightController(DataContext context, ILogger<FlightController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("pilot/profile/{userId}")]
        public async Task<IActionResult> GetPilotProfile(string userId)
        {
            try
            {
                var pilot = await _context.Pilots
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (pilot == null)
                {
                    return NotFound(new { success = false, message = "Pilot profile not found" });
                }

                return Ok(new
                {
                    success = true,
                    id = pilot.Id,
                    userId = pilot.UserId,
                    email = pilot.User?.Email,
                    name = $"{pilot.User?.FirstName} {pilot.User?.LastName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pilot profile");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("pilot/{pilotId}")]
        public async Task<IActionResult> GetPilotFlights(int pilotId)
        {
            try
            {
                var flights = await _context.Flights
                    .Include(f => f.Route)
                    .Include(f => f.AssignedMainPilotNavigation)
                        .ThenInclude(p => p.User)
                    .Include(f => f.AssignedPilotNavigation)
                        .ThenInclude(p => p.User)
                    .Where(f => f.AssignedPilot == pilotId || f.AssignedMainPilot == pilotId)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Route != null ? f.Route.LandingAirport : "No Destination",
                        startingAirport = f.Route != null ? f.Route.TakeoffAirport : "TBD",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft,
                        status = f.Status,
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                        assignedMainPilotId = f.AssignedMainPilot,
                        assignedPilotId = f.AssignedPilot,
                        repeatIntervalHours = f.RepeatIntervalHours,
                        mainPilotName = f.AssignedMainPilotNavigation != null && f.AssignedMainPilotNavigation.User != null
                            ? $"{f.AssignedMainPilotNavigation.User.FirstName} {f.AssignedMainPilotNavigation.User.LastName}"
                            : null,
                        coPilotName = f.AssignedPilotNavigation != null && f.AssignedPilotNavigation.User != null
                            ? $"{f.AssignedPilotNavigation.User.FirstName} {f.AssignedPilotNavigation.User.LastName}"
                            : null,
                        pilotName = f.AssignedMainPilotNavigation != null && f.AssignedMainPilotNavigation.User != null
                            ? $"Main: {f.AssignedMainPilotNavigation.User.FirstName} {f.AssignedMainPilotNavigation.User.LastName}" +
                              (f.AssignedPilotNavigation != null && f.AssignedPilotNavigation.User != null
                                  ? $" | Co-Pilot: {f.AssignedPilotNavigation.User.FirstName} {f.AssignedPilotNavigation.User.LastName}"
                                  : "")
                            : f.AssignedPilotNavigation != null && f.AssignedPilotNavigation.User != null
                                ? $"Co-Pilot: {f.AssignedPilotNavigation.User.FirstName} {f.AssignedPilotNavigation.User.LastName}"
                                : "Not Assigned"
                    })
                    .ToListAsync();

                return Ok(new { success = true, count = flights.Count, flights });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pilot flights");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassignedFlights()
        {
            try
            {
                var flights = await _context.Flights
                    .Include(f => f.Route)
                    .Where(f => f.AssignedPilot == null && f.AssignedMainPilot == null)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Route != null ? f.Route.LandingAirport : "No Destination",
                        startingAirport = f.Route != null ? f.Route.TakeoffAirport : "TBD",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft,
                        status = f.Status,
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                        pilotName = "Not Assigned"
                    })
                    .ToListAsync();

                return Ok(new { success = true, count = flights.Count, flights });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unassigned flights");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("accept/{flightId}")]
        public async Task<IActionResult> AcceptFlight(int flightId, [FromBody] AcceptFlightRequest request)
        {
            try
            {
                var pilot = await _context.Pilots.FirstOrDefaultAsync(p => p.UserId == request.UserId);
                if (pilot == null)
                {
                    return NotFound(new { success = false, message = "Pilot not found" });
                }

                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                bool isMainPilot = request.Role?.ToLower() == "main" || request.Role?.ToLower() == "captain";

                if (isMainPilot)
                {
                    if (flight.AssignedMainPilot != null)
                    {
                        return BadRequest(new { success = false, message = "Main pilot already assigned" });
                    }
                    flight.AssignedMainPilot = pilot.Id;
                }
                else
                {
                    if (flight.AssignedPilot != null)
                    {
                        return BadRequest(new { success = false, message = "Co-pilot already assigned" });
                    }
                    flight.AssignedPilot = pilot.Id;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Flight accepted as {(isMainPilot ? "main pilot" : "co-pilot")}",
                    flightId = flight.Id,
                    pilotId = pilot.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting flight");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPut("{flightId}")]
        public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] UpdateFlightRequest request)
        {
            try
            {
                var flight = await _context.Flights
                    .Include(f => f.Route)
                    .FirstOrDefaultAsync(f => f.Id == flightId);

                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                if (!string.IsNullOrEmpty(request.Aircraft))
                {
                    flight.Aircraft = request.Aircraft;
                }

                if (!string.IsNullOrEmpty(request.StartingAirport) && flight.Route != null)
                {
                    flight.Route.TakeoffAirport = request.StartingAirport;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Flight updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("decline/{flightId}")]
        public async Task<IActionResult> DeclineFlight(int flightId, [FromBody] DeclineFlightRequest request)
        {
            try
            {
                var pilot = await _context.Pilots.FirstOrDefaultAsync(p => p.UserId == request.UserId);
                if (pilot == null)
                {
                    return NotFound(new { success = false, message = "Pilot not found" });
                }

                var flight = await _context.Flights
                    .Include(f => f.Route)
                    .Include(f => f.Baggages)
                    .FirstOrDefaultAsync(f => f.Id == flightId);

                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                bool isMainPilot = flight.AssignedMainPilot == pilot.Id;
                bool isCoPilot = flight.AssignedPilot == pilot.Id;

                if (!isMainPilot && !isCoPilot)
                {
                    return BadRequest(new { success = false, message = "You are not assigned to this flight" });
                }

                if (isMainPilot)
                    flight.AssignedMainPilot = null;
                if (isCoPilot)
                    flight.AssignedPilot = null;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "You have been removed from this flight"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining flight");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }

    public class AcceptFlightRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class UpdateFlightRequest
    {
        public string? Aircraft { get; set; }
        public string? StartingAirport { get; set; }
    }

    public class DeclineFlightRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? DeclineType { get; set; }
    }
}