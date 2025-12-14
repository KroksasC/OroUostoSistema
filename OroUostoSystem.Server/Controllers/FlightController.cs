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

        // GET pilot profile by UserId (converts AspNetUsers.Id → Pilots.Id)
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

        // GET flights for specific pilot
        [HttpGet("pilot/{pilotId}")]
        public async Task<IActionResult> GetPilotFlights(int pilotId)
        {
            try
            {
                var flights = await _context.Flights
                    .Include(f => f.Routes)
                    .Where(f => f.AssignedMainPilot == pilotId || f.AssignedPilot == pilotId)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Routes.Any() ? f.Routes.First().LandingAirport : "No Destination",
                        startingAirport = f.Routes.Any() ? f.Routes.First().TakeoffAirport.ToString() : "TBD",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft,
                        status = f.Status,
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                        routesCount = f.Routes.Count,
                        assignedMainPilotId = f.AssignedMainPilot,
                        assignedPilotId = f.AssignedPilot,
                        repeatIntervalHours = f.RepeatIntervalHours
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

        // GET unassigned flights (both pilot slots empty)
        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassignedFlights()
        {
            try
            {
                var flights = await _context.Flights
                    .Include(f => f.Routes)
                    .Where(f => f.AssignedMainPilot == null && f.AssignedPilot == null)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Routes.Any() ? f.Routes.First().LandingAirport : "No Destination",
                        startingAirport = f.Routes.Any() ? f.Routes.First().TakeoffAirport.ToString() : "TBD",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft,
                        status = f.Status,
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                        routesCount = f.Routes.Count,
                        repeatIntervalHours = f.RepeatIntervalHours
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

        // POST accept flight
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

                // Assign to appropriate role
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
                    assignedMainPilotId = flight.AssignedMainPilot,
                    assignedPilotId = flight.AssignedPilot
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting flight");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // PUT update flight
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

                // Update aircraft
                if (!string.IsNullOrEmpty(request.Aircraft))
                {
                    flight.Aircraft = request.Aircraft;
                }

                // Update starting airport (runway number in route)
                if (request.StartingAirport.HasValue && flight.Routes.Any())
                {
                    flight.Routes.First().TakeoffAirport = request.StartingAirport.Value;
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

        // POST decline flight
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
                    .Include(f => f.Routes)
                    .Include(f => f.Baggages)
                    .FirstOrDefaultAsync(f => f.Id == flightId);

                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if pilot is assigned to this flight
                bool isMainPilot = flight.AssignedMainPilot == pilot.Id;
                bool isCoPilot = flight.AssignedPilot == pilot.Id;

                if (!isMainPilot && !isCoPilot)
                {
                    return BadRequest(new { success = false, message = "You are not assigned to this flight" });
                }

                // Handle repeating vs one-time flight
                if (flight.RepeatIntervalHours.HasValue && request.DeclineType == "once")
                {
                    // Create a one-time copy of this flight for the current date
                    var oneTimeFlight = new Flight
                    {
                        AssignedPilot = flight.AssignedPilot,
                        AssignedMainPilot = flight.AssignedMainPilot,
                        WorkingHours = flight.WorkingHours,
                        FlightDate = flight.FlightDate,
                        Aircraft = flight.Aircraft,
                        FlightNumber = flight.FlightNumber + "-ONCE",
                        Status = flight.Status,
                        RepeatIntervalHours = null // One-time only
                    };

                    // Remove pilot from the one-time copy
                    if (isMainPilot)
                        oneTimeFlight.AssignedMainPilot = null;
                    if (isCoPilot)
                        oneTimeFlight.AssignedPilot = null;

                    _context.Flights.Add(oneTimeFlight);
                    await _context.SaveChangesAsync();

                    // Copy routes for the one-time flight
                    foreach (var route in flight.Routes)
                    {
                        var newRoute = new Route
                        {
                            LandingAirport = route.LandingAirport,
                            Distance = route.Distance,
                            TakeoffAirport = route.TakeoffAirport,
                            Duration = route.Duration,
                            Altitude = route.Altitude,
                            FlightId = oneTimeFlight.Id
                        };
                        _context.Routes.Add(newRoute);
                    }

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "You have been removed from this single occurrence. The repeating flight continues.",
                        declineType = "once",
                        newFlightId = oneTimeFlight.Id
                    });
                }
                else
                {
                    // Decline completely - remove pilot from flight
                    if (isMainPilot)
                        flight.AssignedMainPilot = null;
                    if (isCoPilot)
                        flight.AssignedPilot = null;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = request.DeclineType == "permanent" 
                            ? "You have been permanently removed from this repeating flight"
                            : "You have been removed from this flight",
                        declineType = request.DeclineType ?? "complete"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining flight");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class AcceptFlightRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? Role { get; set; } // "main" or "co-pilot"
    }

    public class UpdateFlightRequest
    {
        public string? Aircraft { get; set; }
        public string? StartingAirport { get; set; }
    }

    public class DeclineFlightRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? DeclineType { get; set; } // "once", "permanent", or null for one-time flights
    }
}