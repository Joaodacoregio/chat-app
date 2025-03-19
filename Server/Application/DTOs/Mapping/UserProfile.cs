using AutoMapper;
using chatApp.Server.Application.DTOs;
using chatApp.Server.Domain.Models;



namespace chatApp.Server.Application 
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // De DTO para Entidade (para criar usuário)
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Ignoramos password pq ele será hash depois

            // De Entidade para DTO de leitura
            CreateMap<User, UserReadDto>();

            // De DTO para Entidade (login)
            CreateMap<UserLoginRequestDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // A senha será validada diretamente, sem mapeamento direto

            // De Entidade para DTO de resposta (login)
            CreateMap<User, UserLoginResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore()); // Token gerado separadamente no controller
        }
    }
}

