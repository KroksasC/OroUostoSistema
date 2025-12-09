using Microsoft.AspNetCore.Identity;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Utility;

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

            // If Admin role exists, assume everything initialized
            if (_context.Roles.Any(x => x.Name == Helper.Admin))
                return;

            // CREATE ROLES
            _roleManager.CreateAsync(new IdentityRole(Helper.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.RegisteredUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Pilot)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Client)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Worker)).GetAwaiter().GetResult();

            // CREATE ADMIN
            var admin = new User
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };
            _userManager.CreateAsync(admin, "Admin123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(admin, Helper.Admin).GetAwaiter().GetResult();

            // CREATE REGISTERED USER
            var user = new User
            {
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                EmailConfirmed = true
            };
            _userManager.CreateAsync(user, "User123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, Helper.RegisteredUser).GetAwaiter().GetResult();

            // CREATE PILOT
            var pilot = new User
            {
                UserName = "pilot@gmail.com",
                Email = "pilot@gmail.com",
                EmailConfirmed = true
            };
            _userManager.CreateAsync(pilot, "Pilot123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(pilot, Helper.Pilot).GetAwaiter().GetResult();

            // CREATE CLIENT
            var client = new User
            {
                UserName = "client@gmail.com",
                Email = "client@gmail.com",
                EmailConfirmed = true
            };
            _userManager.CreateAsync(client, "Client123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(client, Helper.Client).GetAwaiter().GetResult();

            // CREATE WORKER
            var worker = new User
            {
                UserName = "worker@gmail.com",
                Email = "worker@gmail.com",
                EmailConfirmed = true
            };
            _userManager.CreateAsync(worker, "Worker123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(worker, Helper.Worker).GetAwaiter().GetResult();
        }
    }
}
