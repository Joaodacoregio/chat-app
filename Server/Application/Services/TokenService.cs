using chatApp.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using chatApp.Server.Domain.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace chatApp.Server.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ??
                throw new InvalidOperationException("JWT SecretKey not found!");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Nickname ?? user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Reduzi para 15 minutos para access token
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber); // Gera um refresh token seguro
            }
        }

        //Valida o action token caso o usuario tiver o refresh token valido
        public bool ValidateToken(string token, out JwtSecurityToken? jwtToken)
        {
            jwtToken = null;
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ??
                    throw new InvalidOperationException("JWT SecretKey not found!");
                var key = Encoding.UTF8.GetBytes(secretKey);

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                jwtToken = validatedToken as JwtSecurityToken;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal DecodeJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                ?? "Usuário Desconhecido";
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? "ID Desconhecido";

            var claims = jwtToken.Claims.Concat(new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId)
            });

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        public async Task<bool> ValidateRefreshTokenAsync(User user, string refreshToken)
        {
            if (user == null || string.IsNullOrEmpty(refreshToken))
                return false;

            // Verifica se o refresh token corresponde ao armazenado e não expirou
            return user.RefreshToken == refreshToken && user.RefreshTokenExpireTime > DateTime.UtcNow;
        }
    }
}