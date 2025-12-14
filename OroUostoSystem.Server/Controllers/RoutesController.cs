using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models.DTO;

namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RoutesController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ===================================================
        // 1. GET ALL ROUTES
        // ===================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
        {
            var routes = await _context.Routes
                .Include(r => r.Flights)
                .Include(r => r.WeatherForecasts)
                .ToListAsync();

            return Ok(_mapper.Map<List<RouteDto>>(routes));
        }

        // ===================================================
        // 2. GET BY ID
        // ===================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> Get(int id)
        {
            var route = await _context.Routes
                .Include(r => r.Flights)
                .Include(r => r.WeatherForecasts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            return Ok(_mapper.Map<RouteDto>(route));
        }

        // ===================================================
        // 3. UPDATE ROUTE
        // ===================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RouteUpdateDto dto)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound();

            _mapper.Map(dto, route);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===================================================
        // 4. DELETE ROUTE
        // ===================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound();

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
