namespace OroUostoSystem.Server.Models.DTO
{
    public class RouteCreateDto
    {
        public string TakeoffAirport { get; set; } = string.Empty;
        public string LandingAirport { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Duration { get; set; }
        public double Altitude { get; set; }
    }
}
