using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using chatApp.Server.Domain.Models;
using chatApp.Server.Application.DTOs;
using AutoMapper;

namespace chatApp.Server.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
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

                var userExists = await _userManager.FindByEmailAsync(dto.Email);

                if (userExists != null)
                {
                    return StatusCode(500, new { message = "Usuario ja existe!"});
                }

                // Mapeando o DTO para a entidade User
                var user = _mapper.Map<User>(dto);
                user.UserName = dto.Email; // O Identity usa UserName como identificador único, geralmente o email

                // Criando o usuário com o Identity
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    return Ok("Usuário registrado com sucesso.");
                }

                // Se houver erros, retorna os detalhes
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Erro ao registrar usuário", errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao registrar usuário", error = ex.Message });
            }
        }
    }
}