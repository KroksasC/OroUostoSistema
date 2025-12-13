using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;
using System.Text.Json;

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
    // 6. GET LIVE REAL-TIME LOCATION (external system)
    // ===================================================
    // Used in Location modal's map component
    [HttpGet("{id}/location")]
    public async Task<ActionResult<BaggageLiveLocationDto>> GetLiveLocation(int id)
    {
        var baggage = await _context.Baggages.FindAsync(id);
        if (baggage == null)
            return NotFound("Baggage not found.");

        const string url = "https://opensky-network.org/api/states/all";

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

        int index = id % data.States.Count;
        var aircraft = data.States[index];

        // 5 = longitude, 6 = latitude
        JsonElement lonElement = (JsonElement)aircraft[5];
        JsonElement latElement = (JsonElement)aircraft[6];

        if (lonElement.ValueKind != JsonValueKind.Number ||
            latElement.ValueKind != JsonValueKind.Number)
        {
            return StatusCode(500, "External API returned invalid coordinate format.");
        }

        double longitude = lonElement.GetDouble();
        double latitude = latElement.GetDouble();

        var liveDto = new BaggageLiveLocationDto
        {
            Latitude = latitude,
            Longitude = longitude,
            UpdatedAt = DateTime.UtcNow
        };

        return Ok(liveDto);
    }



}
