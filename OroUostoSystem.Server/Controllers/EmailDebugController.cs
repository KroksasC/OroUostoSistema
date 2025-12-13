// .Server/Controllers/EmailDebugController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace OroUostoSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailDebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailDebugController> _logger;
        
        public EmailDebugController(IConfiguration configuration, ILogger<EmailDebugController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        [HttpGet("test-smtp")]
        public async Task<IActionResult> TestSmtpConnection()
        {
            var results = new List<string>();
            
            // Get SMTP settings
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = _configuration.GetValue<int>("SmtpSettings:Port", 587);
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            
            results.Add($"SMTP Host: '{smtpHost}'");
            results.Add($"SMTP Port: {smtpPort}");
            results.Add($"SMTP Username: '{username}'");
            results.Add($"SMTP Password: {(string.IsNullOrEmpty(password) ? "EMPTY" : "SET")}");
            
            // Check if configured
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
            {
                results.Add("❌ SMTP NOT CONFIGURED - Add to appsettings.json");
                return Ok(new { Status = "Not Configured", Details = results });
            }
            
            // Try to send test email
            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = false, // Mailtrap doesn't need SSL
                    Timeout = 5000
                };
                
                var message = new MailMessage
                {
                    From = new MailAddress("test@airline.com"),
                    Subject = "SMTP Test",
                    Body = "This is a test email from Oro Uosto System",
                    IsBodyHtml = false
                };
                
                message.To.Add("test@example.com"); // Any email works
                
                await client.SendMailAsync(message);
                results.Add("✅ SMTP CONNECTION SUCCESSFUL!");
                results.Add($"Email sent via {smtpHost}");
                
                return Ok(new { Status = "Success", Details = results });
            }
            catch (Exception ex)
            {
                results.Add($"❌ SMTP FAILED: {ex.Message}");
                if (ex.InnerException != null)
                {
                    results.Add($"Inner: {ex.InnerException.Message}");
                }
                return Ok(new { Status = "Failed", Details = results });
            }
        }
        
        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string toEmail = "test@example.com")
        {
            try
            {
                // Simple SMTP test without EmailService
                var smtpHost = _configuration["SmtpSettings:Host"];
                var username = _configuration["SmtpSettings:Username"];
                
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
                {
                    return BadRequest("SMTP not configured in appsettings.json");
                }
                
                using var client = new SmtpClient(smtpHost, 2525)
                {
                    Credentials = new NetworkCredential(
                        _configuration["SmtpSettings:Username"],
                        _configuration["SmtpSettings:Password"]),
                    EnableSsl = false
                };
                
                var message = new MailMessage
                {
                    From = new MailAddress("noreply@airline.com", "Oro Uosto System"),
                    Subject = "✈️ TEST Email from System",
                    Body = $"<h2>Test Email</h2><p>Sent at {DateTime.Now}</p>",
                    IsBodyHtml = true
                };
                
                message.To.Add(toEmail);
                
                await client.SendMailAsync(message);
                
                return Ok(new { 
                    Success = true, 
                    Message = $"Test email sent to {toEmail}",
                    Time = DateTime.Now 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    Success = false, 
                    Error = ex.Message,
                    Details = ex.ToString() 
                });
            }
        }
    }
}