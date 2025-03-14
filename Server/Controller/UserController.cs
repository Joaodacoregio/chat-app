using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Data;  // Acesse seu contexto de dados
using chatApp.Server.Models; // Para o modelo de dados de usuário
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace chatApp.Server.Controllers
{
    //Classe para gerenciar operações relacionadas ao usuario , registro  , atualização de perfil e etc...
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Ja é registrado automaticamente
        private readonly IAppDbContext _context;

        public UserController(IAppDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JsonElement data)
        {
            string? email = data.GetProperty("Email").GetString();
            string? password = data.GetProperty("Password").GetString();
            string? nickname = data.GetProperty("Nickname").GetString();
            string? img = data.TryGetProperty("Img", out var imgProperty) ? imgProperty.GetString() : null;

            // Verificação dos campos obrigatórios
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
            {
                return BadRequest("Email, senha e nickname são obrigatórios.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                return BadRequest("Email já registrado.");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var user = new User
            {
                Email = email,
                Password = hashedPassword,
                Nickname = nickname,
                Img = img
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }

    }
}
