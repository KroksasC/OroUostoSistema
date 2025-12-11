using Microsoft.AspNetCore.Identity;

namespace OroUostoSystem.Server.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PersonalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Navigation
        public Client? ClientProfile { get; set; }
        public Employee? EmployeeProfile { get; set; }
        public Pilot? PilotProfile { get; set; }
    }
}
