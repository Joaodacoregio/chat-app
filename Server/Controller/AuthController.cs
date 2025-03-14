using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Data;  // Acesse seu contexto de dados
using chatApp.Server.Models; // Para o modelo de dados de usuário
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace chatApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAppDbContext _context;

        //Ja é registrado automaticamente 
        private readonly IConfiguration _configuration;  

        public AuthController(IAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
 

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token não fornecido." });
            }

            // Decodifica e obtem os dados
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // TODO:MELHORAR A VERIFICAÇÃO POIS EXISTE UMA FALHA DE SEGURANÇA
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Token expirado." });
            }

            return Ok(new { message = "Usuário autenticado." });
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

            //Cookie
            setTokenCookie(token);

            return Ok(new { Token = token });
        }

        // Método para gerar o JWT token
        private string GenerateJwtToken(User user)
        {
            //Pega a chave secreta
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Nickname), // Apelido do usuário
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID do usuário
                new Claim(ClaimTypes.Email, user.Email), // Email do usuário
                new Claim(ClaimTypes.Role, user.Role) // Role do usuário
            };

            // Hash na chave secreta
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Criar o token JWT
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"], // Emissor do token (ex: "MyApp")
                audience: jwtSettings["Audience"], // Público do token (ex: "MyAppClients")
                claims: claims, // Claims do token (informações do usuário)
                expires: DateTime.UtcNow.AddHours(2), // Expiração do token (2 horas)
                signingCredentials: credentials // Credenciais de assinatura
            );

            // Retornar o token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, //JS não pode manipular nem acessar o token
                Secure = false, //Https
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("authToken", token, cookieOptions);
        }

    }
}
