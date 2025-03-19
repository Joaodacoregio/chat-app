using AutoMapper;
using chatApp.Server.Application.DTOs;
using chatApp.Server.Domain.Models;

namespace chatApp.Server.Application
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // De DTO para Entidade (registro de usuário)
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Identity usa UserName como identificador único
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname));
            // Não mapeamos PasswordHash aqui, pois o UserManager.CreateAsync lida com a senha diretamente

            // De Entidade para DTO de leitura
            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname));

            // De Entidade para DTO de resposta (login)
            CreateMap<User, UserLoginResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname));
            // O campo Token será preenchido manualmente no controller após a geração
        }
    }
}