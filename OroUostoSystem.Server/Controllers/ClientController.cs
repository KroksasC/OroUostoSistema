using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly DataContext _context;

    public ClientController(DataContext context)
    {
        _context = context;
    }

    // GET: api/clients
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _context.Clients
            .Include(c => c.User)
            .Select(c => new
            {
                id = c.Id,
                firstName = c.User.FirstName,
                lastName = c.User.LastName
            })
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("byUser/{userId}")]
    public async Task<ActionResult<int>> GetClientId(string userId)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);

        if (client == null)
            return NotFound("Client profile not found");

        return Ok(client.Id);
    }

}
