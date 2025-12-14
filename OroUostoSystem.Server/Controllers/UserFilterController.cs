using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using System.Security.Claims;


namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserFilterController : ControllerBase
    {
        private readonly DataContext _context;

        public UserFilterController(DataContext context)
        {
            _context = context;
        }

        // GET: api/users
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isWorker = await _context.Employees.AnyAsync(e => e.UserId == userId);

            if (!isWorker)
                return Forbid();

            var users = await _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }


        // DELETE: api/users/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _context.Users
                .Include(u => u.ClientProfile)
                    .ThenInclude(c => c.Baggages)
                        .ThenInclude(b => b.Tracking)
                .Include(u => u.ClientProfile)
                    .ThenInclude(c => c.ServiceOrders)
                .Include(u => u.EmployeeProfile)
                .Include(u => u.PilotProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            if (user.ClientProfile != null)
            {
                foreach (var bag in user.ClientProfile.Baggages)
                {
                    if (bag.Tracking != null)
                        _context.BaggageTrackings.Remove(bag.Tracking);
                }

                _context.Baggages.RemoveRange(user.ClientProfile.Baggages);
                _context.ServiceOrders.RemoveRange(user.ClientProfile.ServiceOrders);
                _context.Clients.Remove(user.ClientProfile);
            }

            if (user.EmployeeProfile != null)
                _context.Employees.Remove(user.EmployeeProfile);

            if (user.PilotProfile != null)
                _context.Pilots.Remove(user.PilotProfile);

            var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
            _context.UserRoles.RemoveRange(userRoles);

            var userClaims = _context.UserClaims.Where(uc => uc.UserId == userId);
            _context.UserClaims.RemoveRange(userClaims);

            var userLogins = _context.UserLogins.Where(ul => ul.UserId == userId);
            _context.UserLogins.RemoveRange(userLogins);

            var userTokens = _context.UserTokens.Where(ut => ut.UserId == userId);
            _context.UserTokens.RemoveRange(userTokens);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return NoContent();
        }       


    }
}
