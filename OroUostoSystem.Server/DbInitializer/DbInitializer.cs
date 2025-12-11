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
            //  SEED WORKER EMPLOYEE
            // ==========================================
            if (!_context.Employees.Any())
            {
                var workerUser = await _userManager.FindByEmailAsync("worker@gmail.com");

                var employee = new Employee
                {
                    UserId = workerUser.Id,
                    WorkEmail = "worker@gmail.com",
                    Position = "Ground Staff",
                    HireDate = DateTime.Now.AddYears(-2),
                    Status = "Active",
                    WorkPhone = "+37060000001"
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }
            // ==========================================
            //  SEED SERVICES
            // ==========================================
            if (!_context.Services.Any())
            {
                // Get any existing employee to assign
                var employee = _context.Employees.FirstOrDefault();

                if (employee != null)
                {
                    var service = new Service
                    {
                        Title = "Aircraft Cleaning",
                        Price = 150.00,
                        Category = "Maintenance",
                        Description = "Detailed interior and exterior aircraft cleaning.",
                        EmployeeId = employee.Id
                    };

                    _context.Services.Add(service);
                    await _context.SaveChangesAsync();
                }
            }
            // ==========================================
            //  SEED SERVICE ORDERS
            // ==========================================
            if (!_context.ServiceOrders.Any())
            {
                var client = _context.Clients.FirstOrDefault();
                var service = _context.Services.FirstOrDefault();

                if (client != null && service != null)
                {
                    var order1 = new ServiceOrder
                    {
                        OrderDate = DateTime.Now.AddDays(-2),
                        Quantity = 1,
                        TotalPrice = service.Price,
                        ClientId = client.Id,
                        ServiceId = service.Id
                    };

                    var order2 = new ServiceOrder
                    {
                        OrderDate = DateTime.Now.AddDays(-1),
                        Quantity = 2,
                        TotalPrice = service.Price * 2,
                        ClientId = client.Id,
                        ServiceId = service.Id
                    };

                    _context.ServiceOrders.AddRange(order1, order2);
                    await _context.SaveChangesAsync();
                }
            }

            // ==========================================
            //  SEED FLIGHTS
            // ==========================================
            if (!_context.Flights.Any())
            {
                var flight1 = new Flight
                {
                    AssignedPilot = true,
                    AssignedMainPilot = false,
                    WorkingHours = 3.5f,
                    FlightDate = DateTime.Now.AddDays(-1),
                    Aircraft = "Airbus A320",
                    FlightNumber = "FL1001",
                    Status = "Arrived"
                };

                var flight2 = new Flight
                {
                    AssignedPilot = true,
                    AssignedMainPilot = true,
                    WorkingHours = 2.2f,
                    FlightDate = DateTime.Now,
                    Aircraft = "Boeing 737",
                    FlightNumber = "FL2002",
                    Status = "Boarding"
                };

                _context.Flights.AddRange(flight1, flight2);
                await _context.SaveChangesAsync();
            }

            // ==========================================
            //  SEED BAGGAGE
            // ==========================================
            if (!_context.Baggages.Any())
            {
                var client = _context.Clients.First();
                var flights = _context.Flights.Take(2).ToList();

                var baggage1 = new Baggage
                {
                    Weight = 23.5,
                    RegistrationDate = DateTime.Now.AddHours(-5),
                    Comment = "Blue suitcase with stickers",
                    Size = "Medium",
                    ClientId = client.Id,
                    FlightId = flights[0].Id
                };

                var baggage2 = new Baggage
                {
                    Weight = 18.2,
                    RegistrationDate = DateTime.Now.AddHours(-2),
                    Comment = "Red backpack",
                    Size = "Small",
                    ClientId = client.Id,
                    FlightId = flights[1].Id
                };

                _context.Baggages.AddRange(baggage1, baggage2);
                await _context.SaveChangesAsync();
            }

            // ==========================================
            //  SEED BAGGAGE TRACKING
            // ==========================================
            if (!_context.BaggageTrackings.Any())
            {
                var baggage = _context.Baggages.ToList();

                var tracking = new List<BaggageTracking>
                {
                    new(){
                        BaggageId = baggage[0].Id,
                        Time = DateTime.Now.AddHours(-4),
                        Location = "Check-in Zone A",
                        Status = "Checked-In"
                    },
                    new(){
                        BaggageId = baggage[0].Id,
                        Time = DateTime.Now.AddHours(-3),
                        Location = "Sorting Facility",
                        Status = "In Transit"
                    },
                    new(){
                        BaggageId = baggage[1].Id,
                        Time = DateTime.Now.AddHours(-1),
                        Location = "Gate 12",
                        Status = "Loaded onto Aircraft"
                    }
                };

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
