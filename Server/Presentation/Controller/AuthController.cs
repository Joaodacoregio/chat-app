using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using chatApp.Server.Domain.Interfaces.Services;
using chatApp.Server.Domain.Interfaces.UoW;

namespace chatApp.Server.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly ITokenKeeper _tokenKeeper;
        private readonly IMapper _mapper;

        public AuthController
            (
            IUnitOfWork unityOfWork,
            ITokenService tokenService,
            ITokenKeeper saveTokenService,
            IMapper mapper
            )
        {
            _uow = unityOfWork;
            _tokenService = tokenService;
            _tokenKeeper = saveTokenService;
            _mapper = mapper;
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
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Dados inválidos.");
                }

                var user = await _uow.Users.GetUserByEmailAsync(model.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Unauthorized("Credenciais inválidas.");
                }

                // GERA O TOKEN USANDO O TOKEN SERVICE
                var token = _tokenService.GenerateToken(user);

                // SALVA O TOKEN
                _tokenKeeper.Save(token, Response);

                var authResponse = _mapper.Map<UserLoginResponseDto>(user);
                authResponse.Token = token;

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                // Logar o erro seria interessante aqui, caso tenha um logger
                return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
            }
        }
    }
}
