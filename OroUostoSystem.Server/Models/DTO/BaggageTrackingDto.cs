namespace OroUostoSystem.Server.Models.DTO
{
    public class BaggageTrackingDto
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

}
