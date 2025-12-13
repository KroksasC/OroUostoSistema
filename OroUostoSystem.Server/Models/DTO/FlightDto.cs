namespace OroUostoSystem.Server.Models.DTO
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = "";
        public string Aircraft { get; set; } = "";
        public DateTime FlightDate { get; set; }
        public string Status { get; set; } = "";
        public string? Runway { get; set; }
        public string PilotName { get; set; } = "";
        public string PilotEmail { get; set; } = "";
        public double HoursUntil { get; set; }
        public bool IsSoon { get; set; }
        public string Destination { get; set; } = "";
        public int RoutesCount { get; set; }
    }
}