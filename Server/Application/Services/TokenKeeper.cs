using chatApp.Server.Domain.Interfaces.Services;

namespace chatApp.Server.Application.Services
{
    public class TokenKeeper : ITokenKeeper
    {
        //Vem da interface para salvar o token (cookie , localstorage ou sessionStorage)
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
