using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Domain.Models;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Domain.Interfaces.Bases;
using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;

namespace chatApp.Server.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        // VERIFICAÇÃO SE USUÁRIO ESTÁ AUTENTICADO
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token não fornecido." });
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Validação simples baseada na expiração
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return Unauthorized(new { message = "Token expirado." });
                }

                return Ok(new { message = "Usuário autenticado." });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Token inválido." });
            }
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Dados inválidos.");
            }

            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // GERA O TOKEN USANDO O TOKEN SERVICE
            var token = _tokenService.GenerateToken(user);
            SetTokenCookie(token);

            return Ok(new { Token = token });
        }

        // MÉTODO PRIVADO: SETAR COOKIE (Opcional)
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Defina como true em produção (https)
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("authToken", token, cookieOptions);
        }
    }
}
