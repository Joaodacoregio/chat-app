using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Domain.Models;
using System.Text.Json;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Domain.Interfaces.UoW;


namespace chatApp.Server.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Agora da para usar essa interface para fazer o controle direto com o DB
        private readonly IUnitOfWork _uow;

        public UserController(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JsonElement data)
        {
            try
            {
                // Pegando dados do JSON
                string? email = data.GetProperty("Email").GetString();
                string? password = data.GetProperty("Password").GetString();
                string? nickname = data.GetProperty("Nickname").GetString();
                string? img = data.TryGetProperty("Img", out var imgProperty) ? imgProperty.GetString() : null;

                // Verificação dos campos obrigatórios
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
                {
                    return BadRequest("Email, senha e nickname são obrigatórios.");
                }

                // Verificando se já existe um usuário
                var existingUser = await _uow.Users.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    return BadRequest("Email já registrado.");
                }

                // Hash da senha
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

                var user = new User
                {
                    Email = email,
                    Password = hashedPassword,
                    Nickname = nickname,
                    Img = img
                };

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
