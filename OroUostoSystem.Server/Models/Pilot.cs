using OroUostoSystem.Server.Models;

public class Pilot
{
    public int Id { get; set; }

    public DateTime DaysOff { get; set; }
    public DateTime VacationStart { get; set; }
    public DateTime VacationEnd { get; set; }
    public float MissingWorkHours { get; set; }

    // FK - User
    public string UserId { get; set; }
    public User User { get; set; }

    // Navigation
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
