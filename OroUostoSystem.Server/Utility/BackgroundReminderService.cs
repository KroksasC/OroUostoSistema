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
                    await CheckFlightRemindersFromDatabaseAsync();
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
                // Get flights with BOTH pilot navigations and their User information
                var flights = await dbContext.Flights
                    .Include(f => f.AssignedMainPilotNavigation)     // Include Main Pilot
                        .ThenInclude(p => p.User)                    // Include User from Main Pilot
                    .Include(f => f.AssignedPilotNavigation)         // Include Co-Pilot
                        .ThenInclude(p => p.User)                    // Include User from Co-Pilot
                    .Include(f => f.Routes)                          // Include Routes for destination
                    .Where(f => (f.AssignedMainPilot != null || f.AssignedPilot != null) && // Has at least one pilot
                               f.FlightDate > now &&                 // Future flight
                               f.Status != "Completed" && 
                               f.Status != "Cancelled")
                    .OrderBy(f => f.FlightDate)
                    .ToListAsync();
                
                _logger.LogInformation($"📊 Found {flights.Count} upcoming flights with pilots");
                
                int remindersToSend = 0;
                int emailsSent = 0;
                
                foreach (var flight in flights)
                {
                    var hoursUntilFlight = (flight.FlightDate - now).TotalHours;
                    
                    // Check if flight is within 24 hours
                    if (hoursUntilFlight <= 24 && hoursUntilFlight > 0)
                    {
                        // Get destination from Routes if available
                        var destination = flight.Routes.FirstOrDefault()?.LandingAirport ?? "Unknown";
                        
                        // Send reminder to MAIN PILOT if assigned
                        if (flight.AssignedMainPilotNavigation != null && 
                            flight.AssignedMainPilotNavigation.User != null)
                        {
                            var result = await SendReminderToPilotAsync(
                                flight, 
                                flight.AssignedMainPilotNavigation, 
                                "Main Pilot", 
                                destination
                            );
                            
                            remindersToSend += result.remindersToSend;
                            emailsSent += result.emailsSent;
                        }
                        
                        // Send reminder to CO-PILOT if assigned
                        if (flight.AssignedPilotNavigation != null && 
                            flight.AssignedPilotNavigation.User != null)
                        {
                            var result = await SendReminderToPilotAsync(
                                flight, 
                                flight.AssignedPilotNavigation, 
                                "Co-Pilot", 
                                destination
                            );
                            
                            remindersToSend += result.remindersToSend;
                            emailsSent += result.emailsSent;
                        }
                    }
                }
                
                // Log summary
                _logger.LogInformation($"📈 Summary: {remindersToSend} needed reminders, {emailsSent} emails sent");
                
                // Show next 5 flights (all, not just needing reminders)
                var nextFlights = flights.Take(5).ToList();
                if (nextFlights.Any())
                {
                    _logger.LogInformation("🛫 Next 5 upcoming flights:");
                    foreach (var flight in nextFlights)
                    {
                        var hours = (flight.FlightDate - now).TotalHours;
                        var mainPilotName = flight.AssignedMainPilotNavigation?.User != null 
                            ? $"{flight.AssignedMainPilotNavigation.User.FirstName} {flight.AssignedMainPilotNavigation.User.LastName}"
                            : "No main pilot";
                        var coPilotName = flight.AssignedPilotNavigation?.User != null 
                            ? $"{flight.AssignedPilotNavigation.User.FirstName} {flight.AssignedPilotNavigation.User.LastName}"
                            : "No co-pilot";
                        var destination = flight.Routes.FirstOrDefault()?.LandingAirport ?? "Unknown";
                        
                        _logger.LogInformation($"   {flight.FlightNumber}: {flight.Aircraft} to {destination} in {hours:F1}h");
                        _logger.LogInformation($"      Main: {mainPilotName}");
                        _logger.LogInformation($"      Co-Pilot: {coPilotName}");
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
        
        private async Task<(int remindersToSend, int emailsSent)> SendReminderToPilotAsync(
            Flight flight, 
            Pilot pilot, 
            string pilotRole, 
            string destination)
        {
            int remindersToSend = 0;
            int emailsSent = 0;
            
            var userEmail = pilot.User?.Email;
            var userName = pilot.User != null 
                ? $"{pilot.User.FirstName} {pilot.User.LastName}"
                : "Unknown Pilot";
            
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning($"⚠️ Flight {flight.FlightNumber}: {pilotRole} has no email!");
                return (remindersToSend, emailsSent);
            }
            
            remindersToSend++;
            
            var now = DateTime.UtcNow;
            var hoursUntilFlight = (flight.FlightDate - now).TotalHours;
            
            _logger.LogInformation($"📧 REMINDER NEEDED for flight {flight.FlightNumber}");
            _logger.LogInformation($"   Aircraft: {flight.Aircraft}");
            _logger.LogInformation($"   Date: {flight.FlightDate:yyyy-MM-dd HH:mm}");
            _logger.LogInformation($"   Hours until: {hoursUntilFlight:F1}h");
            _logger.LogInformation($"   {pilotRole}: {userName} ({userEmail})");
            _logger.LogInformation($"   Status: {flight.Status}");
            _logger.LogInformation($"   Destination: {destination}");
            
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
                    _logger.LogInformation($"✅ Email sent to {pilotRole} for flight {flight.FlightNumber}");
                }
                else
                {
                    _logger.LogWarning($"❌ Failed to send email to {pilotRole} for flight {flight.FlightNumber}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error sending email to {pilotRole} for flight {flight.FlightNumber}");
            }
            
            return (remindersToSend, emailsSent);
        }
    }
}