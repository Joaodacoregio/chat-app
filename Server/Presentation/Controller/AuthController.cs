using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Domain.Interfaces.Bases;
using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;
using chatApp.Server.Domain.Interfaces.Services;

namespace chatApp.Server.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenKeeper _tokenKeeper;

        public AuthController(IUserRepository userRepository, ITokenService tokenService, ITokenKeeper saveTokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _tokenKeeper = saveTokenService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
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

                // SALVA O TOKEN
                _tokenKeeper.Save(token, Response);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Logar o erro seria interessante aqui, caso tenha um logger
                return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
            }
        }

    }
}
