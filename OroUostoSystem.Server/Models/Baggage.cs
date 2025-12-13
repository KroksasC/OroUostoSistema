public class Baggage
{
    public int Id { get; set; }

    public double Weight { get; set; }

    public DateTime RegistrationDate { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;

    // FK — Client
    public int ClientId { get; set; }
    public Client Client { get; set; }

    // FK — Flight
    public int FlightId { get; set; }
    public Flight Flight { get; set; }

    // Navigation
    public BaggageTracking? Tracking { get; set; }
}
