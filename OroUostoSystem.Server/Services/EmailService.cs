using System.Net;
using System.Net.Mail;

namespace OroUostoSystem.Server.Services
{
    public class EmailService
    {
        private readonly string _email;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _email = configuration["EmailSettings:Email"];
            _password = configuration["EmailSettings:Password"];
        }

        public async Task SendEmailForServiceOrder(string emailToSend, string serviceName, double price)
        {
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_email, _password)
            };

            var email = BuildEmailForServiceOrder(emailToSend, serviceName, price);

            try
            {
                await smtp.SendMailAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        private MailMessage BuildEmailForServiceOrder(string emailToSend, string serviceName, double price)
        {
            var email = new MailMessage();
            email.From = new MailAddress(_email);
            email.To.Add(emailToSend);

            email.Subject = "Your Service Order Confirmation";

            email.Body =
                $"Thank you for ordering the service '{serviceName}'.\n\n" +
                $"Total price: {price} €\n" +
                $"Your order has been successfully registered.\n\n" +
                "Best regards,\nAirport Service Team";

            return email;
        }
    }
}
