using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Models;

namespace OroUostoSystem.Server.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Baggage> Baggages { get; set; }
        public DbSet<BaggageTracking> BaggageTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // -----------------------------
            //  Client ↔ User (1:1)
            // -----------------------------
            builder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne(u => u.ClientProfile)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  Employee ↔ User (1:1)
            // -----------------------------
            builder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.EmployeeProfile)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  Pilot ↔ User (1:1)
            // -----------------------------
            builder.Entity<Pilot>()
                .HasOne(p => p.User)
                .WithOne(u => u.PilotProfile)
                .HasForeignKey<Pilot>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  Flight ↔ Pilot (nullable foreign keys)
            // -----------------------------
            // For AssignedPilot (co-pilot)
            builder.Entity<Flight>()
                .HasOne(f => f.AssignedPilotNavigation)
                .WithMany()
                .HasForeignKey(f => f.AssignedPilot)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false); // Make it nullable

            // For AssignedMainPilot (main pilot)
            builder.Entity<Flight>()
                .HasOne(f => f.AssignedMainPilotNavigation)
                .WithMany()
                .HasForeignKey(f => f.AssignedMainPilot)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false); // Make it nullable

            // -----------------------------
            //  Flight ↔ Routes (many : 1)
            // -----------------------------
            builder.Entity<Flight>()
                .HasOne(f => f.Route)
                .WithMany(r => r.Flights)
                .HasForeignKey(f => f.RouteId)
                .OnDelete(DeleteBehavior.SetNull);

            // -----------------------------
            //  WeatherForecast ↔ Route (many : 1)
            // -----------------------------
            builder.Entity<WeatherForecast>()
                .HasOne(w => w.Route)
                .WithMany(r => r.WeatherForecasts)
                .HasForeignKey(w => w.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            //  ServiceOrder ↔ Client (many : 1)
            // -----------------------------
            builder.Entity<ServiceOrder>()
                .HasOne(o => o.Client)
                .WithMany()
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  ServiceOrder ↔ Service (many : 1)
            // -----------------------------
            builder.Entity<ServiceOrder>()
                .HasOne(o => o.Service)
                .WithMany(s => s.ServiceOrders)
                .HasForeignKey(o => o.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            //  Service ↔ Employee (many : 1)
            // -----------------------------
            builder.Entity<Service>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Services)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  Baggage ↔ Client (many : 1)
            // -----------------------------
            builder.Entity<Baggage>()
                .HasOne(b => b.Client)
                .WithMany(c => c.Baggages)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            //  BaggageTracking ↔ Baggage (1 : 1)
            // -----------------------------
            builder.Entity<Baggage>()
                .HasOne(b => b.Tracking)
                .WithOne(t => t.Baggage)
                .HasForeignKey<BaggageTracking>(t => t.BaggageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}