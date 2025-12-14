using OroUostoSystem.Server;

public class Route
{
    public int Id { get; set; }

    public string TakeoffAirport { get; set; } = string.Empty;
    public string LandingAirport { get; set; } = string.Empty;
    public string TakeoffAirport { get; set; } = string.Empty;
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double Altitude { get; set; }

    // Navigation
    public ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
