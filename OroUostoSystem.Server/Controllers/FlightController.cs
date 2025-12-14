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

        public FlightController(DataContext context)
        {
            _context = context;
        }

        // TEST 1: Simplest possible endpoint
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok(new { message = "FlightController exists!" });
        }

        // TEST 2: Database connection
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                var count = await _context.Flights.CountAsync();
                return Ok(new
                {
                    success = true,
                    message = "Database connected!",
                    flightCount = count
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // TEST 3: Get actual flights
        [HttpGet("pilot-flights")]
        public async Task<IActionResult> GetPilotFlights()
        {
            try
            {
                var flights = await _context.Flights
                    .Include(f => f.Route)
                    .Take(10)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Route != null ? f.Route.LandingAirport : "No Destination",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft ?? "Unknown",
                        pilotName = "Sample Pilot",
                        status = f.Status ?? "Scheduled",
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = flights.Count,
                    flights = flights
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // TEST 4: Get flights WITHOUT routes (if routes cause issues)
        [HttpGet("simple")]
        public async Task<IActionResult> GetSimpleFlights()
        {
            try
            {
                var flights = await _context.Flights
                    .Take(10)
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft,
                        status = f.Status
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = flights.Count,
                    flights = flights
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // GET UNASSIGNED FLIGHTS (for recommendations)
        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassignedFlights()
        {
            try
            {
                var unassignedFlights = await _context.Flights
                    .Include(f => f.Route)
                    .Where(f => f.PilotId == null) // Flights without assigned pilot
                    .Select(f => new
                    {
                        id = f.Id,
                        flightId = f.FlightNumber,
                        destination = f.Route != null ? f.Route.LandingAirport : "No Destination",
                        startingAirport = f.Route != null ? f.Route.TakeoffAirport : "TBD",
                        takeOffTime = f.FlightDate,
                        planeName = f.Aircraft ?? "Unknown",
                        status = f.Status ?? "Scheduled",
                        isSoon = (f.FlightDate - DateTime.UtcNow).TotalHours < 24,
                        hoursUntil = (f.FlightDate - DateTime.UtcNow).TotalHours,
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = unassignedFlights.Count,
                    flights = unassignedFlights
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // ACCEPT FLIGHT - Assign pilot to flight
        [HttpPost("accept/{flightId}")]
        public async Task<IActionResult> AcceptFlight(int flightId, [FromBody] AcceptFlightRequest request)
        {
            try
            {
                var flight = await _context.Flights.FindAsync(flightId);
                
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                if (flight.PilotId != null)
                {
                    return BadRequest(new { success = false, message = "Flight already assigned to another pilot" });
                }

                // Find pilot by UserId
                var pilot = await _context.Pilots
                    .FirstOrDefaultAsync(p => p.UserId == request.UserId);

                if (pilot == null)
                {
                    return NotFound(new { success = false, message = "Pilot profile not found" });
                }

                flight.PilotId = pilot.Id;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Flight accepted successfully",
                    flightId = flight.Id,
                    pilotId = pilot.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // UPDATE FLIGHT - Edit aircraft and starting airport
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

                // Update starting airport (in the route)
                if (request.StartingAirport != null && flight.Route != null)
                {
                    var route = flight.Route;
                    route.TakeoffAirport = request.StartingAirport;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Flight updated successfully",
                    flight = new
                    {
                        id = flight.Id,
                        aircraft = flight.Aircraft,
                        startingAirport = flight.Route?.TakeoffAirport ?? "TBD"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }

    // Request models
    public class AcceptFlightRequest
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateFlightRequest
    {
        public string? Aircraft { get; set; }
        public string? StartingAirport { get; set; }
    }
}
