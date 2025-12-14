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

                var admin = await _userManager.FindByEmailAsync("admin@gmail.com");
                admin.FirstName = "System";
                admin.LastName = "Administrator";
                await _userManager.UpdateAsync(admin);

                var regUser = await _userManager.FindByEmailAsync("user@gmail.com");
                regUser.FirstName = "Regular";
                regUser.LastName = "User";
                await _userManager.UpdateAsync(regUser);

                var pilot = await _userManager.FindByEmailAsync("pilot@gmail.com");
                pilot.FirstName = "John";
                pilot.LastName = "Eagle";
                await _userManager.UpdateAsync(pilot);

                var clientt = await _userManager.FindByEmailAsync("client@gmail.com");
                clientt.FirstName = "Maria";
                clientt.LastName = "Kowalski";
                await _userManager.UpdateAsync(clientt);

                var worker = await _userManager.FindByEmailAsync("worker@gmail.com");
                worker.FirstName = "David";
                worker.LastName = "Clark";
                await _userManager.UpdateAsync(worker);
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
            //  SEED PILOTS
            // ==========================================
            if (!_context.Pilots.Any())
            {
                var pilotUser = await _userManager.FindByEmailAsync("pilot@gmail.com");

                var pilot = new Pilot
                {
                    UserId = pilotUser.Id,
                    DaysOff = DateTime.Now.AddDays(30),
                    VacationStart = DateTime.Now.AddMonths(2),
                    VacationEnd = DateTime.Now.AddMonths(2).AddDays(14),
                    MissingWorkHours = 0
                };

                _context.Pilots.Add(pilot);
                await _context.SaveChangesAsync();
            }
            // ==========================================
            //  SEED SERVICES
            // ==========================================
            if (!_context.Services.Any())
            {
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
            //  SEED ROUTES
            // ==========================================
            if (!_context.Routes.Any())
            {
                var routes = new List<Route>
                {
                    new(){
                        TakeoffAirport = "KUN",
                        LandingAirport = "VNO",
                        Distance = 5540,
                        Duration = 7.08,
                        Altitude = 10.7
                    },
                    new(){
                        TakeoffAirport = "KUN",
                        LandingAirport = "PLQ",
                        Distance = 8770,
                        Duration = 11.5,
                        Altitude = 11.0
                    },
                    new(){
                        TakeoffAirport = "JFK",
                        LandingAirport = "LAX",
                        Distance = 3983,
                        Duration = 5.5,
                        Altitude = 11.3
                    },
                    new(){
                        TakeoffAirport = "ORD",
                        LandingAirport = "MIA",
                        Distance = 1882,
                        Duration = 3.2,
                        Altitude = 10.5
                    },
                    new(){
                        TakeoffAirport = "DFW",
                        LandingAirport = "SEA",
                        Distance = 2794,
                        Duration = 4.1,
                        Altitude = 10.8
                    },
                    new(){
                        TakeoffAirport = "ATL",
                        LandingAirport = "DEN",
                        Distance = 1862,
                        Duration = 3.5,
                        Altitude = 10.9
                    },
                    new(){
                        TakeoffAirport = "BOS",
                        LandingAirport = "SFO",
                        Distance = 4341,
                        Duration = 6.2,
                        Altitude = 11.2
                    },
                    new(){
                        TakeoffAirport = "PHX",
                        LandingAirport = "MSP",
                        Distance = 2043,
                        Duration = 3.3,
                        Altitude = 10.6
                    },
                    new(){
                        TakeoffAirport = "LAS",
                        LandingAirport = "MCO",
                        Distance = 3236,
                        Duration = 4.8,
                        Altitude = 10.7
                    },
                    new(){
                        TakeoffAirport = "PDX",
                        LandingAirport = "IAH",
                        Distance = 3054,
                        Duration = 4.5,
                        Altitude = 11.1
                    }
                };

                _context.Routes.AddRange(routes);
                await _context.SaveChangesAsync();
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
                var pilots = _context.Pilots.ToList();
                var pilotId = pilots.FirstOrDefault()?.Id;
                
                var routes = _context.Routes.ToList();
                if (routes.Count >= 3)
                {
                    var flights = new List<Flight>
                    {
                        new(){
                            RouteId = routes[0].Id,
                            AssignedPilot = null,
                            AssignedMainPilot = pilotId,
                            WorkingHours = 3.5f,
                            FlightDate = DateTime.Now.AddDays(-1),
                            Aircraft = "Airbus A320",
                            FlightNumber = "FL1001",
                            Status = "Arrived"
                        },
                        new(){
                            RouteId = routes[1].Id,
                            AssignedPilot = pilotId,
                            AssignedMainPilot = null,
                            WorkingHours = 2.2f,
                            FlightDate = DateTime.Now,
                            Aircraft = "Boeing 737",
                            FlightNumber = "FL2002",
                            Status = "Boarding"
                        },
                        new(){
                            RouteId = routes[2].Id,
                            AssignedPilot = null,
                            AssignedMainPilot = null,
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
                        Latitude = 40 + random.NextDouble() * 20,
                        Longitude = -5 + random.NextDouble() * 15
                    });
                }

                _context.BaggageTrackings.AddRange(tracking);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateUserWithRole(string email, string password, string role)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, password);
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
