using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OroUostoSystem.Server.Models.DTO;

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
    // UserFilter.jsx
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _context.Clients
            .Include(c => c.User)
            .Select(c => new
            {
                id = c.Id,
                firstName = c.User.FirstName,
                lastName = c.User.LastName,
                email = c.User.Email
            })
            .ToListAsync();

        return Ok(clients);
    }
    // GET: api/clients/byUser/{userId}
    // UserProfile.jsx
    [HttpGet("byUser/{userId}")]
    public async Task<ActionResult<int>> GetClientId(string userId)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);

        if (client == null)
            return NotFound("Client profile not found");

        return Ok(client.Id);
    }

    // PUT: api/clients/me
    // UserProfile.jsx
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateClientDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var client = await _context.Clients
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (client == null)
            return NotFound("Client not found");

        client.User.FirstName = dto.FirstName;
        client.User.LastName = dto.LastName;
        client.User.Email = dto.Email;
        client.User.PersonalCode = dto.PersonalCode;
        client.User.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/clients/{clientId}
    // AdminClientManagement.jsx
    [HttpDelete("{clientId}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound("Client not found");

        _context.Users.Remove(client.User);
        _context.Clients.Remove(client);

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
