using chatApp.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace chatApp.Server.Services
{
    public class TokenService : ITokenService, ISaveToken 
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //out -> não precisa ser inicializado antes de ser passado para o método.
        public bool ValidateToken(string token, out JwtSecurityToken jwtToken)
        {
            jwtToken = null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var parsedToken = handler.ReadJwtToken(token);

                if (parsedToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                jwtToken = parsedToken;
                return true;
            }
            catch
            {
                return false;
            }
        }


        public void Save(string token, HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Defina como true em produção (https)
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            response.Cookies.Append("authToken", token, cookieOptions);

        }
    }
}
