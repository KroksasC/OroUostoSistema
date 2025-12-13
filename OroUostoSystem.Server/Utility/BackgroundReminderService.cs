using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Services;

namespace OroUostoSystem.Server.Utility
{
    public class BackgroundReminderService : BackgroundService
    {
        private readonly ILogger<BackgroundReminderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailService _emailService;
        
        public BackgroundReminderService(
            ILogger<BackgroundReminderService> logger,
            IServiceProvider serviceProvider,
            EmailService emailService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _emailService = emailService;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Flight Reminder Service Started!");

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                try
                {
                    await CheckFlightRemindersFromDatabaseAsync(); // ← No parameter
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error checking flight reminders");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        
        private async Task CheckFlightRemindersFromDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var now = DateTime.UtcNow;
            _logger.LogInformation($"⏰ [{now:HH:mm:ss}] Checking flight reminders...");
            
            try
            {
                // Get flights with their Pilot and User information
                var flights = await dbContext.Flights
                    .Include(f => f.Pilot)               // Include Pilot
                        .ThenInclude(p => p.User)        // Include User from Pilot
                    .Include(f => f.Routes)              // Include Routes for destination
                    .Where(f => f.PilotId != null &&     // Has assigned pilot
                               f.FlightDate > now &&     // Future flight
                               f.Status != "Completed" && 
                               f.Status != "Cancelled")
                    .OrderBy(f => f.FlightDate)
                    .ToListAsync();
                
                _logger.LogInformation($"📊 Found {flights.Count} upcoming flights with pilots");
                
                int remindersToSend = 0;
                int emailsSent = 0;
                var flightsNeedingReminders = new List<(Flight flight, string email, string name)>();
                
                foreach (var flight in flights)
                {
                    var hoursUntilFlight = (flight.FlightDate - now).TotalHours;
                    
                    // Check if flight is within 24 hours
                    if (hoursUntilFlight <= 24 && hoursUntilFlight > 0)
                    {
                        // Get email and name from User via Pilot
                        var userEmail = flight.Pilot?.User?.Email;
                        var userName = flight.Pilot?.User != null 
                            ? $"{flight.Pilot.User.FirstName} {flight.Pilot.User.LastName}"
                            : "Unknown Pilot";
                        
                        if (string.IsNullOrEmpty(userEmail))
                        {
                            _logger.LogWarning($"⚠️ Flight {flight.FlightNumber}: Pilot has no email!");
                            continue;
                        }
                        
                        remindersToSend++;
                        flightsNeedingReminders.Add((flight, userEmail, userName));
                        
                        _logger.LogInformation($"📧 REMINDER NEEDED for flight {flight.FlightNumber}");
                        _logger.LogInformation($"   Aircraft: {flight.Aircraft}");
                        _logger.LogInformation($"   Date: {flight.FlightDate:yyyy-MM-dd HH:mm}");
                        _logger.LogInformation($"   Hours until: {hoursUntilFlight:F1}h");
                        _logger.LogInformation($"   Pilot: {userName} ({userEmail})");
                        _logger.LogInformation($"   Status: {flight.Status}");
                        
                        // Get destination from Routes if available
                        var destination = "Unknown";
                        if (flight.Routes.Any())
                        {
                            var route = flight.Routes.First();
                            destination = route.LandingAirport;
                            _logger.LogInformation($"   Destination: {destination}");
                        }
                        else
                        {
                            _logger.LogInformation($"   Destination: Unknown (no route data)");
                        }
                        
                        // ============================================
                        // SEND ACTUAL EMAIL HERE
                        // ============================================
                        try
                        {
                            var emailSent = await _emailService.SendFlightReminderAsync(
                                userEmail,
                                userName,
                                flight.FlightNumber,
                                flight.Aircraft,
                                flight.FlightDate,
                                destination
                            );
                            
                            if (emailSent)
                            {
                                emailsSent++;
                                _logger.LogInformation($"✅ Email sent for flight {flight.FlightNumber}");
                                
                                // Optional: Mark flight as reminded in database
                                // Uncomment when you add ReminderSent field to Flight model
                                /*
                                flight.ReminderSent = true;
                                flight.ReminderSentAt = now;
                                dbContext.Flights.Update(flight);
                                */
                            }
                            else
                            {
                                _logger.LogWarning($"❌ Failed to send email for flight {flight.FlightNumber}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"💥 Error sending email for flight {flight.FlightNumber}");
                        }
                    }
                }
                
                // Save changes to database if we marked any flights as reminded
                // Uncomment when you add ReminderSent field
                /*
                if (emailsSent > 0)
                {
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"💾 Saved {emailsSent} flight reminder statuses to database");
                }
                */
                
                // Log summary
                _logger.LogInformation($"📈 Summary: {remindersToSend} needed reminders, {emailsSent} emails sent");
                
                if (flightsNeedingReminders.Any())
                {
                    _logger.LogInformation("📋 Flights processed:");
                    foreach (var (flight, email, name) in flightsNeedingReminders)
                    {
                        var hours = (flight.FlightDate - now).TotalHours;
                        _logger.LogInformation($"   • {flight.FlightNumber}: {flight.Aircraft}, {hours:F1}h until, Pilot: {name}");
                    }
                }
                else
                {
                    _logger.LogInformation("✅ No flights need reminders right now");
                }
                
                // Show next 5 flights (all, not just needing reminders)
                var nextFlights = flights.Take(5).ToList();
                if (nextFlights.Any())
                {
                    _logger.LogInformation("🛫 Next 5 upcoming flights:");
                    foreach (var flight in nextFlights)
                    {
                        var hours = (flight.FlightDate - now).TotalHours;
                        var pilotName = flight.Pilot?.User != null 
                            ? $"{flight.Pilot.User.FirstName} {flight.Pilot.User.LastName}"
                            : "No pilot";
                        var destination = flight.Routes.FirstOrDefault()?.LandingAirport ?? "Unknown";
                        
                        _logger.LogInformation($"   {flight.FlightNumber}: {flight.Aircraft} to {destination} in {hours:F1}h - {pilotName}");
                    }
                }
                else
                {
                    _logger.LogInformation("📭 No upcoming flights found");
                }
                
                _logger.LogInformation("✅ Reminder check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Database error in reminder service");
            }
        }
    }
}