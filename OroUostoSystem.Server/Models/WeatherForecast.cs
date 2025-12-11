public class WeatherForecast
{
    public int Id { get; set; }

    public double Humidity { get; set; }
    public double Temperature { get; set; }
    public DateTime CheckTime { get; set; }
    public double WindSpeed { get; set; }

    // FK — Route
    public int RouteId { get; set; }
    public Route Route { get; set; }
}
