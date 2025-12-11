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

        public DBInitializer(
            DataContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            await _context.Database.MigrateAsync();

            // ==========================================
            //  SEED ROLES (only if empty)
            // ==========================================
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.RegisteredUser));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Pilot));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Client));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Worker));
            }

            // ==========================================
            //  SEED USERS (only if empty)
            // ==========================================
            if (!_userManager.Users.Any())
            {
                await CreateUserWithRole("admin@gmail.com", "Admin123!", Helper.Admin);
                await CreateUserWithRole("user@gmail.com", "User123!", Helper.RegisteredUser);
                await CreateUserWithRole("pilot@gmail.com", "Pilot123!", Helper.Pilot);
                await CreateUserWithRole("client@gmail.com", "Client123!", Helper.Client);
                await CreateUserWithRole("worker@gmail.com", "Worker123!", Helper.Worker);
            }

            // ==========================================
            //  SEED CLIENTS
            // ==========================================
            if (!_context.Clients.Any())
            {
                var clientUser = await _userManager.FindByEmailAsync("client@gmail.com");

                var client = new Client
                {
                    UserId = clientUser.Id,
                    BirthDate = new DateTime(1990, 5, 10),
                    LoyaltyLevel = "Silver",
                    Points = 1200,
                    RegistrationDate = DateTime.Now.AddYears(-1)
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            // ==========================================
            //  SEED FLIGHTS
            // ==========================================
            if (!_context.Flights.Any())
            {
                var flights = new List<Flight>
                {
                    new(){
                        AssignedPilot = true,
                        AssignedMainPilot = false,
                        WorkingHours = 3.5f,
                        FlightDate = DateTime.Now.AddDays(-1),
                        Aircraft = "Airbus A320",
                        FlightNumber = "FL1001",
                        Status = "Arrived"
                    },
                    new(){
                        AssignedPilot = true,
                        AssignedMainPilot = true,
                        WorkingHours = 2.2f,
                        FlightDate = DateTime.Now,
                        Aircraft = "Boeing 737",
                        FlightNumber = "FL2002",
                        Status = "Boarding"
                    },
                    new(){
                        AssignedPilot = false,
                        AssignedMainPilot = false,
                        WorkingHours = 4.1f,
                        FlightDate = DateTime.Now.AddDays(1),
                        Aircraft = "Embraer 190",
                        FlightNumber = "FL3003",
                        Status = "Scheduled"
                    }
                };

                _context.Flights.AddRange(flights);
                await _context.SaveChangesAsync();
            }


            // ==========================================
            //  SEED BAGGAGE
            // ==========================================
            if (!_context.Baggages.Any())
            {
                var client = _context.Clients.First();
                var flights = _context.Flights.ToList();

                var baggageList = new List<Baggage>
                {
                    new(){
                        Weight = 23.5,
                        RegistrationDate = DateTime.Now.AddHours(-5),
                        Comment = "Blue suitcase with stickers",
                        Size = "Medium",
                        ClientId = client.Id,
                        FlightId = flights[0].Id
                    },
                    new(){
                        Weight = 18.2,
                        RegistrationDate = DateTime.Now.AddHours(-2),
                        Comment = "Red backpack",
                        Size = "Small",
                        ClientId = client.Id,
                        FlightId = flights[1].Id
                    },
                    new(){
                        Weight = 30.0,
                        RegistrationDate = DateTime.Now.AddHours(-3),
                        Comment = "Large black luggage",
                        Size = "Large",
                        ClientId = client.Id,
                        FlightId = flights[2].Id
                    }
                };

                // ==========================================
                //  ADD 10 EXTRA RANDOM BAGGAGE RECORDS
                // ==========================================
                var random = new Random();
                string[] comments =
                {
                    "Grey Samsonite suitcase",
                    "Hard-shell case with airport stickers",
                    "Small kids backpack with cartoon print",
                    "Black duffel bag",
                    "Sport bag with equipment",
                    "Red suitcase with wheels",
                    "Damaged black bag (handle taped)",
                    "Bright neon green travel bag",
                    "Yellow purse with metal locks",
                    "Checked-in cardboard box"
                };

                string[] sizes = { "Small", "Medium", "Large" };

                for (int i = 0; i < 10; i++)
                {
                    baggageList.Add(new Baggage
                    {
                        Weight = Math.Round(random.NextDouble() * 25 + 5, 1), // 5–30kg
                        RegistrationDate = DateTime.Now.AddHours(-random.Next(1, 12)),
                        Comment = comments[i],
                        Size = sizes[random.Next(sizes.Length)],
                        ClientId = client.Id,
                        FlightId = flights[random.Next(flights.Count)].Id
                    });
                }

                _context.Baggages.AddRange(baggageList);
                await _context.SaveChangesAsync();
            }



            // ==========================================
            //  SEED BAGGAGE TRACKING (1:1)
            // ==========================================
            if (!_context.BaggageTrackings.Any())
            {
                var baggage = _context.Baggages.ToList();
                var random = new Random();

                var tracking = new List<BaggageTracking>();

                foreach (var bag in baggage)
                {
                    tracking.Add(new BaggageTracking
                    {
                        BaggageId = bag.Id,
                        UpdatedAt = DateTime.Now.AddMinutes(-random.Next(10, 600)),
                        Latitude = 40 + random.NextDouble() * 20,     // approx Europe
                        Longitude = -5 + random.NextDouble() * 15
                    });
                }

                _context.BaggageTrackings.AddRange(tracking);
                await _context.SaveChangesAsync();
            }

        }

        // =====================================================
        //  HELPER FOR USER CREATION + ROLE ASSIGNMENT
        // =====================================================
        private async Task CreateUserWithRole(string email, string password, string role)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, password);
            await _context.SaveChangesAsync();    // IMPORTANT for SQLite
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
