using OroUostoSystem.Server;

public class Route
{
    public int Id { get; set; }

    public string LandingAirport { get; set; } = string.Empty;
    public double Distance { get; set; }
    public double TakeoffAirport { get; set; }
    public double Duration { get; set; }
    public double Altitude { get; set; }

    // FK — Flight
    public int FlightId { get; set; }
    public Flight Flight { get; set; }

    // Navigation
    //asdasd
    public ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
}
