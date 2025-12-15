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

        [HttpGet]
        public async Task<IActionResult> GetAllFlights()
        {
            var flights = await _context.Flights
                .Select(f => new
                {
                    id = f.Id,
                    flightNumber = f.FlightNumber,
                    flightDate = f.FlightDate
                })
                .ToListAsync();

            return Ok(flights);
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

[HttpGet("unassigned/{pilotId}")] // Add {pilotId} to the route
public async Task<IActionResult> GetUnassignedFlights(int pilotId)
{
    try
    {
        // Get pilot's missing work hours AND vacation dates
        var pilot = await _context.Pilots.FindAsync(pilotId);
        if (pilot == null)
            return NotFound("Pilot not found");
        
        var missingWorkHours = pilot.MissingWorkHours;
        var vacationStart = pilot.VacationStart; // DateTime, not DateTime?
        var vacationEnd = pilot.VacationEnd;     // DateTime, not DateTime?
        
        // Check if pilot has vacation scheduled (check for default/min date)
        bool hasVacationScheduled = vacationStart > DateTime.MinValue 
            && vacationEnd > DateTime.MinValue 
            && vacationStart <= vacationEnd;
        
        // Check if pilot is currently on vacation
        bool isOnVacation = hasVacationScheduled
            && DateTime.UtcNow >= vacationStart 
            && DateTime.UtcNow <= vacationEnd;
        
        // Get all unassigned flights
        var unassignedFlights = await _context.Flights
            .Where(f => f.AssignedPilot == null || f.AssignedMainPilot == null)
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
                pilotName = "Not Assigned",
                workingHours = f.WorkingHours
            })
            .ToListAsync();
        
        // Filter flights: 
        // 1. Show flights that would help reduce missing hours
        // 2. EXCLUDE flights that occur during pilot's vacation
        var recommendedFlights = unassignedFlights
            .Where(f => f.workingHours < missingWorkHours) // Helps fill missing hours
            .Where(f => {
                // Check if pilot has a vacation scheduled
                if (!hasVacationScheduled)
                    return true; // No vacation scheduled
                
                // Check if flight date overlaps with vacation period
                bool flightDuringVacation = f.takeOffTime >= vacationStart 
                    && f.takeOffTime <= vacationEnd;
                
                return !flightDuringVacation; // Exclude if during vacation
            })
            .ToList();
        
        return Ok(new { 
            success = true, 
            count = recommendedFlights.Count, 
            flights = recommendedFlights,
            pilotMissingHours = missingWorkHours,
            isOnVacation = isOnVacation,
            hasVacationScheduled = hasVacationScheduled,
            vacationPeriod = hasVacationScheduled ? 
                $"{vacationStart:yyyy-MM-dd} to {vacationEnd:yyyy-MM-dd}" : "No vacation scheduled",
            note = $"Showing flights with less than {missingWorkHours} hours, excluding vacation periods"
        });
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