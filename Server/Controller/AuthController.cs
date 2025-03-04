using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Data;  // Acesse seu contexto de dados
using chatApp.Server.Models; // Para o modelo de dados de usuário
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace chatApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Dados inválidos.");
            }

            // Verificar se o email já está cadastrado
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email já registrado.");
            }

            // Fazendo o hash no servidor
            var salt = BCrypt.Net.BCrypt.GenerateSalt(); 
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

            // Criar o novo usuário
            var user = new User
            {
                Email = model.Email,
                Password = hashedPassword,
                Nickname = model.Nickname,
                Img = model.Img
            };

            // Adicionar no banco de dados
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }
    }
}