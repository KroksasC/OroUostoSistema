using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OroUostoSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public RoutesController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AirportRouteSystem/1.0");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
        {
            var routes = await _context.Routes
                .Include(r => r.Flights)
                .Include(r => r.WeatherForecasts)
                .ToListAsync();

            return Ok(_mapper.Map<List<RouteDto>>(routes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> Get(int id)
        {
            var route = await _context.Routes
                .Include(r => r.Flights)
                .Include(r => r.WeatherForecasts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            return Ok(_mapper.Map<RouteDto>(route));
        }

        [HttpPost]
        public async Task<ActionResult<RouteDto>> Create([FromBody] RouteCreateDto dto)
        {
            var route = _mapper.Map<Route>(dto);
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var createdRoute = await _context.Routes
                .Include(r => r.Flights)
                .Include(r => r.WeatherForecasts)
                .FirstOrDefaultAsync(r => r.Id == route.Id);

            return CreatedAtAction(nameof(Get), new { id = route.Id }, _mapper.Map<RouteDto>(createdRoute));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RouteUpdateDto dto)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound();

            _mapper.Map(dto, route);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound();

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/weather")]
        public async Task<ActionResult<WeatherForecastDto>> GetWeather(int id)
        {
            var route = await _context.Routes
                .Include(r => r.WeatherForecasts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            var existingForecast = route.WeatherForecasts.FirstOrDefault();
            if (existingForecast != null)
            {
                return Ok(_mapper.Map<WeatherForecastDto>(existingForecast));
            }

            var (latitude, longitude) = GetCoordinatesFromAirportCode(route.LandingAirport);
            var weatherData = await FetchWeatherFromApi(latitude, longitude);

            var forecast = new WeatherForecast
            {
                RouteId = route.Id,
                Humidity = weatherData.Humidity,
                Temperature = weatherData.Temperature,
                WindSpeed = weatherData.WindSpeed,
                Pressure = weatherData.Pressure,
                CheckTime = DateTime.Now
            };

            _context.WeatherForecasts.Add(forecast);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<WeatherForecastDto>(forecast));
        }

        private (double latitude, double longitude) GetCoordinatesFromAirportCode(string airportCode)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(airportCode));
            
            var latValue = BitConverter.ToUInt64(hashBytes, 0);
            var lonValue = BitConverter.ToUInt64(hashBytes, 8);

            var latitude = 24.5 + (latValue % 10000) / 10000.0 * (49.4 - 24.5);
            var longitude = -125 + (lonValue % 10000) / 10000.0 * (-66 - (-125));

            return (Math.Round(latitude, 4), Math.Round(longitude, 4));
        }

        private async Task<WeatherData> FetchWeatherFromApi(double latitude, double longitude)
        {
            try
            {
                var pointUrl = $"https://api.weather.gov/points/{latitude},{longitude}";
                var pointResponse = await _httpClient.GetStringAsync(pointUrl);
                var pointData = JsonDocument.Parse(pointResponse);
                
                var forecastUrl = pointData.RootElement
                    .GetProperty("properties")
                    .GetProperty("forecast")
                    .GetString();

                var forecastResponse = await _httpClient.GetStringAsync(forecastUrl);
                var forecastData = JsonDocument.Parse(forecastResponse);
                
                var periods = forecastData.RootElement
                    .GetProperty("properties")
                    .GetProperty("periods");

                if (periods.GetArrayLength() > 0)
                {
                    var currentPeriod = periods[0];
                    
                    var tempF = currentPeriod.GetProperty("temperature").GetDouble();
                    var tempC = (tempF - 32) * 5 / 9;
                    
                    var windSpeedStr = currentPeriod.GetProperty("windSpeed").GetString() ?? "0 mph";
                    var windSpeedMph = ParseWindSpeed(windSpeedStr);
                    var windSpeedKmh = windSpeedMph * 1.60934;
                    
                    var humidity = currentPeriod.TryGetProperty("relativeHumidity", out var humidityProp) 
                        ? humidityProp.GetProperty("value").GetDouble() 
                        : 50;

                    var pressure = 1013.25;
                    try
                    {
                        var observationUrl = pointData.RootElement
                            .GetProperty("properties")
                            .GetProperty("observationStations")
                            .GetString();
                        
                        var stationsResponse = await _httpClient.GetStringAsync(observationUrl);
                        var stationsData = JsonDocument.Parse(stationsResponse);
                        var stations = stationsData.RootElement
                            .GetProperty("features");
                        
                        if (stations.GetArrayLength() > 0)
                        {
                            var stationUrl = stations[0]
                                .GetProperty("id")
                                .GetString() + "/observations/latest";
                            
                            var observationResponse = await _httpClient.GetStringAsync(stationUrl);
                            var observationData = JsonDocument.Parse(observationResponse);
                            
                            if (observationData.RootElement
                                .GetProperty("properties")
                                .TryGetProperty("barometricPressure", out var pressureProp))
                            {
                                var pressurePa = pressureProp.GetProperty("value").GetDouble();
                                pressure = pressurePa / 100;
                            }
                        }
                    }
                    catch
                    {
                    }

                    return new WeatherData
                    {
                        Temperature = Math.Round(tempC, 1),
                        Humidity = Math.Round(humidity, 1),
                        WindSpeed = Math.Round(windSpeedKmh, 1),
                        Pressure = Math.Round(pressure, 1)
                    };
                }
            }
            catch
            {
            }

            return new WeatherData
            {
                Temperature = 15,
                Humidity = 60,
                WindSpeed = 20,
                Pressure = 1013.25
            };
        }

        private double ParseWindSpeed(string windSpeedStr)
        {
            var parts = windSpeedStr.Split(' ');
            if (parts.Length > 0 && double.TryParse(parts[0], out var speed))
            {
                return speed;
            }
            return 10;
        }

        private class WeatherData
        {
            public double Temperature { get; set; }
            public double Humidity { get; set; }
            public double WindSpeed { get; set; }
            public double Pressure { get; set; }
        }
    }
}
