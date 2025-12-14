using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Interfaces;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Utility;

namespace OroUostoSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly DataContext _context;

        public AccountController(
            DataContext context,
            UserManager<User> userManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid username or password.");

            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                message = "Login successful",
                username = user.UserName,
                userId = user.Id,
                token,
                role = await _userManager.GetRolesAsync(user)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest("User already exists!");

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest("User creation failed!");

            await _userManager.AddToRoleAsync(user, Helper.Client);

            var client = new Client
            {
                UserId = user.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                LoyaltyLevel = "Bronze",
                Points = 0,
                RegistrationDate = DateTime.Now
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully!");
        }
    }
}