using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.UoW;
using AutoMapper;
using chatApp.Server.Application.DTOs;

namespace chatApp.Server.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                // Verificação dos campos obrigatórios
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Nickname))
                {
                    return BadRequest("Email, senha e nickname são obrigatórios.");
                }

                // Verificando se já existe um usuário
                var existingUser = await _uow.Users.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return BadRequest("Email já registrado.");
                }

                // Hash da senha
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);

                // Mapeando o DTO para a entidade User, e alterando a senha para o valor criptografado
                var user = _mapper.Map<User>(dto);
                user.Password = hashedPassword;  

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                return Ok("Usuário registrado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao registrar usuário", error = ex.Message });
            }
        }
    }
}
