public class BaggageTracking
{
    public int Id { get; set; }

    public DateTime Time { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // FK — Baggage
    public int BaggageId { get; set; }
    public Baggage Baggage { get; set; }
}
