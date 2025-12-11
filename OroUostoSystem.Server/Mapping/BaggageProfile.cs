using AutoMapper;
using OroUostoSystem.Server.Models.DTO;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OroUostoSystem.Server.Mapping
{
    public class BaggageProfile : Profile
    {
        public BaggageProfile()
        {
            CreateMap<Baggage, BaggageDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.User.FirstName + " " + src.Client.User.LastName))
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.Flight.FlightNumber));

            CreateMap<BaggageCreateDto, Baggage>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<BaggageUpdateDto, Baggage>();

            CreateMap<BaggageTracking, BaggageLiveLocationDto>()
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(s => s.UpdatedAt));

        }
    }
}
