using OroUostoSystem.Server.Models;

public class Employee
{
    public int Id { get; set; }

    public string WorkEmail { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string WorkPhone { get; set; } = string.Empty;

    // FK - User
    public string UserId { get; set; }
    public User User { get; set; }

    // Navigation
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
