public class BaggageTracking
{
    public int Id { get; set; }

    public DateTime UpdatedAt { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    // FK — Baggage
    public int BaggageId { get; set; }
    public Baggage Baggage { get; set; }
}
