using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OroUostoSystem.Server.Utility
{
    public class BackgroundReminderService : BackgroundService
    {
        private readonly ILogger<BackgroundReminderService> _logger;
        
        public BackgroundReminderService(ILogger<BackgroundReminderService> logger)
        {
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Background Reminder Service Started!");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in reminder service");
                }
                
                // Check every 30 seconds for testing (change to 1 hour later)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        private async Task CheckRemindersAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation($"⏰ Checking flight reminders at {now:HH:mm:ss}");
            
            // Simple hardcoded test flights
            var testFlights = new[]
            {
                new { Id = 1, FlightId = "F001", Destination = "Paris", TakeOff = now.AddHours(25) },
                new { Id = 2, FlightId = "F002", Destination = "New York", TakeOff = now.AddHours(12) },
                new { Id = 3, FlightId = "F003", Destination = "London", TakeOff = now.AddHours(48) }
            };
            
            foreach (var flight in testFlights)
            {
                var hoursUntil = (flight.TakeOff - now).TotalHours;
                
                if (hoursUntil <= 24 && hoursUntil > 0)
                {
                    _logger.LogInformation($"📧 WOULD send reminder for flight {flight.FlightId} to {flight.Destination}");
                    _logger.LogInformation($"   Takeoff in: {hoursUntil:F1} hours");
                }
            }
            
            _logger.LogInformation("✅ Reminder check complete");
        }
    }
}