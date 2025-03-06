using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Data;  // Acesse seu contexto de dados
using chatApp.Server.Models; // Para o modelo de dados de usuário
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;

namespace chatApp.Server.Controllers
{
    //Não funcional
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Dados inválidos.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // Lógica para gerar o token JWT
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        // Método para gerar o JWT token
        private string GenerateJwtToken(User user)
        {
            return "generated-jwt-token";
        }
    }
}
