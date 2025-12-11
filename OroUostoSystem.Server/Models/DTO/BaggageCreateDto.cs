namespace OroUostoSystem.Server.Models.DTO
{
    public class BaggageCreateDto
    {
        public double Weight { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public int FlightId { get; set; }
    }

}
