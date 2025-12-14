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
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.Flight.FlightNumber))
                .ForMember(dest => dest.LatestForecast, opt => opt.MapFrom(src => 
                    src.WeatherForecasts.OrderByDescending(w => w.CheckTime).FirstOrDefault()));

            CreateMap<WeatherForecast, WeatherForecastDto>();
        }
    }
}
