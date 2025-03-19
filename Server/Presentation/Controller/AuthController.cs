using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.Services;
using System.Security.Claims;

namespace chatApp.Server.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ITokenKeeper _tokenKeeper;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<User> userManager,
            ITokenService tokenService,
            ITokenKeeper saveTokenService,
            IMapper mapper)
        {
            _userManager = userManager;
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

            if (!_tokenService.ValidateToken(token, out JwtSecurityToken? jwtToken))
            {
                return Unauthorized(new { message = "Token inválido ou expirado." });
            }

            return Ok(new { message = "Usuário autenticado." });
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

                // Busca o usuário pelo email usando o UserManager
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Unauthorized("Credenciais inválidas.");
                }

                // Gera o access token e o refresh token
                var accessToken = _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Armazena o refresh token no usuário
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7); // Refresh token válido por 7 dias
                await _userManager.UpdateAsync(user);

                // Salva o access token (ex.: em cookie)
                _tokenKeeper.Save(accessToken, Response);

                // Mapeia a resposta
                var authResponse = _mapper.Map<UserLoginResponseDto>(user);
                authResponse.Token = accessToken;
                authResponse.RefreshToken = refreshToken;

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.AccessToken) || string.IsNullOrEmpty(model.RefreshToken))
                {
                    return BadRequest("Access token e refresh token são obrigatórios.");
                }

                // Decodifica o access token para obter o ID do usuário
                var principal = _tokenService.DecodeJwtToken(model.AccessToken);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized("Token inválido.");
                }

                // Busca o usuário
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !await _tokenService.ValidateRefreshTokenAsync(user, model.RefreshToken))
                {
                    return Unauthorized("Refresh token inválido ou expirado.");
                }

                // Gera um novo access token
                var newAccessToken = _tokenService.GenerateToken(user);
                _tokenKeeper.Save(newAccessToken, Response);

                // Mapeia a resposta
                var authResponse = _mapper.Map<UserLoginResponseDto>(user);
                authResponse.Token = newAccessToken;
                authResponse.RefreshToken = model.RefreshToken; // Mantém o mesmo refresh token

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
            }
        }
    }
}