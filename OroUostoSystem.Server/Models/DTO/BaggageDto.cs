namespace OroUostoSystem.Server.Models.DTO
{
    public class BaggageDto
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;

        public BaggageLiveLocationDto? Tracking { get; set; }
    }

}
