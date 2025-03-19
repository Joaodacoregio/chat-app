using AutoMapper;
using chatApp.Server.Application.DTOs;
using chatApp.Server.Domain.Models;

namespace chatApp.Server.Application
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname));
        }
    }
}
