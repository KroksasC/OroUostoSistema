// .Server/Services/EmailService.cs
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace OroUostoSystem.Server.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        
        public EmailService(
            ILogger<EmailService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        // Keep the flight reminder functionality
        public async Task<bool> SendFlightReminderAsync(
            string toEmail, 
            string pilotName, 
            string flightNumber, 
            string aircraft, 
            DateTime flightDate, 
            string destination)
        {
            try
            {
                // Get configuration - could unify to use either section
            var smtpHost = _configuration["SmtpSettings:FlightReminders:Host"] 
                         ?? _configuration["SmtpSettings:Host"]; // Fallback
            var smtpPort = _configuration.GetValue<int>("SmtpSettings:FlightReminders:Port", 2525);
            var username = _configuration["SmtpSettings:FlightReminders:Username"] 
                         ?? _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:FlightReminders:Password"] 
                         ?? _configuration["SmtpSettings:Password"];
            var fromEmail = _configuration["SmtpSettings:FlightReminders:FromEmail"] 
                          ?? _configuration["SmtpSettings:FromEmail"] 
                          ?? username;
            var enableSsl = _configuration.GetValue<bool>("SmtpSettings:FlightReminders:EnableSsl", false);
                
                // If Host or Username is empty → TEST MODE (log only)
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
                {
                    _logger.LogInformation($"📧 [TEST MODE] Would send flight reminder to: {toEmail}");
                    _logger.LogInformation($"   Subject: ✈️ Flight Reminder: {flightNumber}");
                    await Task.Delay(1);
                    return true;
                }
                
                _logger.LogInformation($"📤 Sending flight reminder to: {toEmail}");
                
                using var client = CreateSmtpClient(smtpHost, smtpPort, username, password, enableSsl);
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Oro Uosto System"),
                    Subject = $"✈️ Flight Reminder: {flightNumber}",
                    Body = CreateFlightReminderBody(pilotName, flightNumber, aircraft, flightDate, destination),
                    IsBodyHtml = true
                };
                
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"✅ Flight reminder sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send flight reminder to {toEmail}");
                return false;
            }
        }
        
        // Add the service order functionality from master branch
        public async Task<bool> SendServiceOrderConfirmationAsync(
            string emailToSend, 
            string serviceName, 
            double price)
        {
            try
            {
                // Get configuration - could unify to use either section
            var smtpHost = _configuration["SmtpSettings:ServiceOrders:Host"] 
                         ?? _configuration["SmtpSettings:Host"]; // Fallback
            var smtpPort = _configuration.GetValue<int>("SmtpSettings:ServiceOrders:Port", 587);
            var username = _configuration["SmtpSettings:ServiceOrders:Username"] 
                         ?? _configuration["EmailSettings:Email"]; // Fallback to old config
            var password = _configuration["SmtpSettings:ServiceOrders:Password"] 
                         ?? _configuration["EmailSettings:Password"];
            var fromEmail = _configuration["SmtpSettings:ServiceOrders:FromEmail"] 
                          ?? _configuration["SmtpSettings:FromEmail"] 
                          ?? username;
            var enableSsl = _configuration.GetValue<bool>("SmtpSettings:ServiceOrders:EnableSsl", true);
        
                
                // If Host or Username is empty → TEST MODE (log only)
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
                {
                    _logger.LogInformation($"📧 [TEST MODE] Would send service confirmation to: {emailToSend}");
                    _logger.LogInformation($"   Service: {serviceName}, Price: {price}€");
                    await Task.Delay(1);
                    return true;
                }
                
                _logger.LogInformation($"📤 Sending service confirmation to: {emailToSend}");
                
                using var client = CreateSmtpClient(smtpHost, smtpPort, username, password, enableSsl);
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Oro Uosto System"),
                    Subject = "Your Service Order Confirmation",
                    Body = CreateServiceOrderBody(serviceName, price),
                    IsBodyHtml = false // Plain text as in master branch
                };
                
                mailMessage.To.Add(emailToSend);
                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"✅ Service confirmation sent successfully to {emailToSend}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send service confirmation to {emailToSend}");
                return false;
            }
        }
        
        // Helper method to create SMTP client
        private SmtpClient CreateSmtpClient(string host, int port, string username, string password, bool enableSsl)
        {
            return new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };
        }
        
        private string CreateFlightReminderBody(string pilotName, string flightNumber, string aircraft, 
            DateTime flightDate, string destination)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #2c3e50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                        .content {{ background: #f8f9fa; padding: 20px; border: 1px solid #dee2e6; border-top: none; border-radius: 0 0 5px 5px; }}
                        .flight-info {{ background: white; padding: 15px; margin: 15px 0; border-left: 4px solid #3498db; }}
                        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #eee; color: #7f8c8d; font-size: 12px; }}
                        .btn {{ display: inline-block; background: #3498db; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>✈️ Flight Reminder</h1>
                    </div>
                    <div class='content'>
                        <p><strong>Dear {pilotName},</strong></p>
                        <p>This is a reminder for your upcoming flight assignment.</p>
                        
                        <div class='flight-info'>
                            <h3 style='margin-top: 0;'>Flight {flightNumber}</h3>
                            <p><strong>🛫 Aircraft:</strong> {aircraft}</p>
                            <p><strong>📍 Destination:</strong> {destination}</p>
                            <p><strong>⏰ Scheduled Takeoff:</strong> {flightDate:dddd, MMMM dd, yyyy HH:mm} UTC</p>
                            <p><strong>⏳ Time until flight:</strong> 24 hours</p>
                        </div>
                        
                        <p><strong>Important Reminders:</strong></p>
                        <ul>
                            <li>Arrive at the airport at least 2 hours before departure</li>
                            <li>Check weather conditions for your route</li>
                            <li>Review flight plan and NOTAMs</li>
                            <li>Ensure all required documentation is prepared</li>
                        </ul>
                        
                        <p>Safe travels!</p>
                        <p><em>The Flight Operations Team</em></p>
                    </div>
                    <div class='footer'>
                        <p>This is an automated reminder from <strong>Oro Uosto System</strong>.</p>
                        <p>If you have any questions, please contact the flight operations department.</p>
                        <p>© {DateTime.Now.Year} Airline System. All rights reserved.</p>
                    </div>
                </body>
                </html>";
        }
        
        private string CreateServiceOrderBody(string serviceName, double price)
        {
            return $@"
Thank you for ordering the service '{serviceName}'.

Total price: {price} €
Your order has been successfully registered.

Best regards,
Airport Service Team";
        }
    }
}