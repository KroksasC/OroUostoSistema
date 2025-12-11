using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly DataContext _context;

    public FlightController(DataContext context)
    {
        _context = context;
    }

    // GET: api/flight
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
}
