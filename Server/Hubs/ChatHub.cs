using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message, string token)
        {
            var user = DecodeJwtToken(token);

            var userName = user?.Identity?.Name ?? "Usuário Desconhecido"; // Caso não haja nome, usa um nome padrão

            // Enviar a mensagem para todos os clientes conectados
            await Clients.All.SendAsync("ReceiveMessage", $"{userName}: {message}");
        }

        // Método para decodificar e validar o token JWT
        private ClaimsPrincipal DecodeJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);

            //Se o nome não existir o usuario é desconhecido 
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Usuário Desconhecido";

            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
        }

    }

}
