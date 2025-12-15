namespace OroUostoSystem.Server.Models.DTO
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string TakeoffAirport { get; set; } = string.Empty;
        public string LandingAirport { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Duration { get; set; }
        public double Altitude { get; set; }
        public List<string> FlightNumbers { get; set; } = new List<string>();
        public WeatherForecastDto? LatestForecast { get; set; }
    }

    public class WeatherForecastDto
    {
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public DateTime CheckTime { get; set; }
        public double WindSpeed { get; set; }
        public double Pressure { get; set; }
    }
}
