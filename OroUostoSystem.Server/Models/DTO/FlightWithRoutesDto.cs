namespace OroUostoSystem.Server.Models.DTO
{
    public class FlightWithRoutesDto : FlightDto
    {
        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();
    }

    public class RouteDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string LandingAirport { get; set; } = "";
        public double Distance { get; set; }
    }
}