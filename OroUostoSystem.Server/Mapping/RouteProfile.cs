using AutoMapper;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;

namespace OroUostoSystem.Server.Mapping
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            CreateMap<Route, RouteDto>()
                .ForMember(dest => dest.FlightNumbers, opt => opt.MapFrom(src => 
                    src.Flights.Select(f => f.FlightNumber).ToList()))
                .ForMember(dest => dest.LatestForecast, opt => opt.MapFrom(src => 
                    src.WeatherForecasts.OrderByDescending(w => w.CheckTime).FirstOrDefault()));

            CreateMap<WeatherForecast, WeatherForecastDto>();

            CreateMap<RouteUpdateDto, Route>();
        }
    }
}
