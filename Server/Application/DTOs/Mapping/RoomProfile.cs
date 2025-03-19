using AutoMapper;
using chatApp.Server.Application.DTOs;
using chatApp.Server.Domain.Models;

namespace chatApp.Server.Application
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {

            CreateMap<Room, RoomDto>()
             .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")));


            CreateMap<RoomValidationDto, Room>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        }
    }
}
