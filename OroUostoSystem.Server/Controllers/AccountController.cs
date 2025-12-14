using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Interfaces;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Utility;

namespace MaistoSistema2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenservive;

        public AccountController(DataContext context,
            UserManager<User> userManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenservive = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password.");
            };

            var token = _tokenservive.CreateToken(user);

            return Ok(new
            {
                message = "Login succesfull",
                username = user.UserName,
                userId = user.Id,
                token = token,
                Role = await _userManager.GetRolesAsync(user)
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
            User user = new()
            {
                Email = model.Email,
                UserName = model.Name,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            _userManager.AddToRoleAsync(user, Helper.RegisteredUser).GetAwaiter().GetResult();
            if (!result.Succeeded)
            {
                return BadRequest("User creation failed! Please check user details and try again.");
            }
            return Ok("User created successfully!");
        }
    }
}
