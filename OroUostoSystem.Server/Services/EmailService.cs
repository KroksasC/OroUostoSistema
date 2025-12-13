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
        // 1. READ CONFIGURATION from appsettings.json
        var smtpHost = _configuration["SmtpSettings:Host"];            // e.g., "smtp.gmail.com"
        var smtpPort = _configuration.GetValue<int>("SmtpSettings:Port"); // e.g., 587
        var username = _configuration["SmtpSettings:Username"];        // Your email
        var password = _configuration["SmtpSettings:Password"];        // App password
        var fromEmail = _configuration["SmtpSettings:FromEmail"];      // Displayed as sender
        var enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl", true);
        
        // 2. CHECK IF CONFIGURED
        // If Host or Username is empty → TEST MODE (log only)
        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
        {
            _logger.LogInformation($"📧 [TEST MODE] Would send to: {toEmail}");
            _logger.LogInformation($"   Subject: ✈️ Flight Reminder: {flightNumber}");
            _logger.LogInformation($"   Body: Dear {pilotName}, your flight {flightNumber} is in 24h");
            await Task.Delay(1);
            return true; // Return true for testing
        }
        
        // 3. LOG REAL SENDING ATTEMPT
        _logger.LogInformation($"📤 Sending REAL email to: {toEmail}");
        _logger.LogInformation($"   Using SMTP: {smtpHost}:{smtpPort}");
        
        // 4. CREATE SMTP CLIENT (like setting up mail truck)
        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(username, password), // Login to email server
            EnableSsl = enableSsl,     // Encrypt connection (true for Gmail)
            DeliveryMethod = SmtpDeliveryMethod.Network, // Send over internet
            Timeout = 10000            // Wait max 10 seconds
        };
        
        // 5. CREATE EMAIL MESSAGE (like writing a letter)
        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "Oro Uosto System"), // Sender address
            Subject = $"✈️ Flight Reminder: {flightNumber}",       // Email subject
            Body = CreateEmailBody(pilotName, flightNumber, aircraft, flightDate, destination),
            IsBodyHtml = true  // Send as HTML (with formatting)
        };
        
        // 6. ADD RECIPIENT (address the envelope)
        mailMessage.To.Add(toEmail);
        
        // Optional: Add CC (carbon copy)
        // mailMessage.CC.Add("admin@airline.com");
        
        // 7. SEND EMAIL (put letter in mailbox)
        await client.SendMailAsync(mailMessage);
        
        // 8. LOG SUCCESS
        _logger.LogInformation($"✅ Email sent successfully to {toEmail}");
        return true;
    }
    catch (Exception ex)
    {
        // 9. HANDLE ERRORS
        _logger.LogError(ex, $"❌ Failed to send email to {toEmail}");
        return false;
    }
        }
        
        private string CreateEmailBody(string pilotName, string flightNumber, string aircraft, 
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
    }
}