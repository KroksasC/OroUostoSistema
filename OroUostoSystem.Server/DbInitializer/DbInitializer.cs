using Microsoft.AspNetCore.Identity;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace OroUostoSystem.Server.DbInitializer
{
    public class DBInitializer : IDbinitializer
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(DataContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Token key is not configured.");
            }

            if (_context.Roles.Any(x => x.Name == Utility.Helper.Admin))
            {
                return;
            }

            _roleManager.CreateAsync(new IdentityRole(Utility.Helper.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Utility.Helper.RegisteredUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            }, "Admin123!").GetAwaiter().GetResult();

            User user = _context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, Utility.Helper.Admin).GetAwaiter().GetResult();

        }
    }
}
