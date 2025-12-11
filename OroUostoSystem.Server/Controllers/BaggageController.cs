using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;

[ApiController]
[Route("api/[controller]")]
public class BaggageController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly HttpClient _http;

    public BaggageController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _http = new HttpClient();
    }

    // ===================================================
    // 1. GET ALL BAGGAGE
    // ===================================================
    // Frontend BaggageList.jsx uses this
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BaggageDto>>> GetAll()
    {
        var baggage = await _context.Baggages
            .Include(b => b.Client).ThenInclude(c => c.User)
            .Include(b => b.Flight)
            .Include(b => b.Tracking)
            .ToListAsync();

        return Ok(_mapper.Map<List<BaggageDto>>(baggage));
    }

    // ===================================================
    // 2. GET BY ID
    // ===================================================
    // Used in Details modal
    [HttpGet("{id}")]
    public async Task<ActionResult<BaggageDto>> Get(int id)
    {
        var baggage = await _context.Baggages
            .Include(b => b.Client).ThenInclude(c => c.User)
            .Include(b => b.Flight)
            .Include(b => b.Tracking)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (baggage == null)
            return NotFound();

        return Ok(_mapper.Map<BaggageDto>(baggage));
    }

    // ===================================================
    // 3. CREATE BAGGAGE
    // ===================================================
    // Used in RegisterLuggage.jsx
    [HttpPost]
    public async Task<ActionResult<BaggageDto>> Create(BaggageCreateDto dto)
    {
        var baggage = _mapper.Map<Baggage>(dto);
        _context.Baggages.Add(baggage);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = baggage.Id }, _mapper.Map<BaggageDto>(baggage));
    }

    // ===================================================
    // 4. UPDATE BAGGAGE
    // ===================================================
    // Used in Search modal (Edit button)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BaggageUpdateDto dto)
    {
        var baggage = await _context.Baggages.FindAsync(id);
        if (baggage == null)
            return NotFound();

        _mapper.Map(dto, baggage);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ===================================================
    // 5. DELETE BAGGAGE
    // ===================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var baggage = await _context.Baggages.FindAsync(id);
        if (baggage == null)
            return NotFound();

        _context.Baggages.Remove(baggage);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ===================================================
    // 6. GET BAGGAGE TRACKING HISTORY
    // ===================================================
    // Used in Location modal (history list)
    [HttpGet("{id}/tracking")]
    public async Task<ActionResult<List<BaggageTrackingDto>>> GetTracking(int id)
    {
        var track = await _context.BaggageTrackings
            .Where(t => t.BaggageId == id)
            .OrderBy(t => t.Time)
            .ToListAsync();

        return Ok(_mapper.Map<List<BaggageTrackingDto>>(track));
    }

    // ===================================================
    // 7. ADD TRACKING ENTRY
    // ===================================================
    [HttpPost("{id}/tracking")]
    public async Task<IActionResult> AddTracking(int id, BaggageTrackingCreateDto dto)
    {
        var baggage = await _context.Baggages.FindAsync(id);
        if (baggage == null)
            return NotFound();

        var tracking = new BaggageTracking
        {
            BaggageId = id,
            Time = DateTime.Now,
            Location = dto.Location,
            Status = dto.Status
        };

        _context.BaggageTrackings.Add(tracking);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<BaggageTrackingDto>(tracking));
    }

    // ===================================================
    // 8. GET LIVE REAL-TIME LOCATION (external system)
    // ===================================================
    // Used in Location modal's map component
    [HttpGet("{id}/location")]
    public async Task<ActionResult<BaggageLiveLocationDto>> GetLiveLocation(int id)
    {
        // 1. verify baggage exists
        var baggage = await _context.Baggages.FindAsync(id);
        if (baggage == null)
            return NotFound("Baggage not found.");

        // 2. external API call
        var url = "https://opensky-network.org/api/states/all";

        OpenSkyResponseDto? data;

        try
        {
            data = await _http.GetFromJsonAsync<OpenSkyResponseDto>(url);
        }
        catch
        {
            return StatusCode(503, "Failed to reach external tracking system.");
        }

        if (data?.States == null || data.States.Count == 0)
            return StatusCode(502, "External tracking system returned no usable data.");

        // 3. pick an aircraft index based on baggage id
        int aircraftIndex = id % data.States.Count;

        var aircraft = data.States[aircraftIndex];

        // OpenSky structure:
        // index 5 = longitude
        // index 6 = latitude
        double? lon = aircraft[5] as double?;
        double? lat = aircraft[6] as double?;

        if (lat == null || lon == null)
            return StatusCode(500, "External API returned invalid coordinate data.");

        // 4. map to our DTO
        var liveDto = new BaggageLiveLocationDto
        {
            Latitude = lat.Value,
            Longitude = lon.Value,
            Status = "Tracked via OpenSky realtime feed",
            UpdatedAt = DateTime.UtcNow
        };

        return Ok(liveDto);
    }

}
