public class Flight
{
    public int Id { get; set; }

    public bool AssignedPilot { get; set; }
    public bool AssignedMainPilot { get; set; }
    public float WorkingHours { get; set; }
    public DateTime FlightDate { get; set; }
    public string Aircraft { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // FK — Pilot
    public int? PilotId { get; set; }
    public Pilot? Pilot { get; set; }

    // FK - Route
    public int? RouteId { get; set; }
    public Route? Route { get; set; }


    // Navigation
    public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();

}
