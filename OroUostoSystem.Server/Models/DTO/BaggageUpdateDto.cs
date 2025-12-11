namespace OroUostoSystem.Server.Models.DTO
{
    public class BaggageUpdateDto
    {
        public double Weight { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;

        public int FlightId { get; set; }
    }

}
