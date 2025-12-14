using OroUostoSystem.Server.Models;

public class Client
{
    public int Id { get; set; }

    public DateTime BirthDate { get; set; }
    public string LoyaltyLevel { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime RegistrationDate { get; set; }

    // FK - User
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; }

    // FK - ServiceOrders
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

    // Navigation
    public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
}
