using chatApp.Server.Models;
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
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();


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


        // Quando um usuário se conecta
        public async Task UserConnected(string token)
        {
            var user = DecodeJwtToken(token);  // Decodificando o token e pegando o nome do usuário
            string userName = user?.Identity?.Name ?? "Usuário Desconhecido";  // Nome do usuário ou "Usuário Desconhecido"

            // Adiciona o novo usuário na lista
            _userConnections[Context.ConnectionId] = userName;

            // Envia para todos os clientes (inclusive o novo) a lista atualizada de usuários conectados
            var connectedUsers = _userConnections.Values.ToList();
            await Clients.All.SendAsync("UserConnected", userName);  // Notifica todos sobre o novo usuário

            // Envia a lista de usuários conectados para o novo usuário
            await Clients.Caller.SendAsync("ConnectedUsers", connectedUsers);  // Envia a lista para o novo usuário
        }

        // Quando um usuário se desconecta
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                var userName = _userConnections[Context.ConnectionId];
                _userConnections.Remove(Context.ConnectionId);  // Remove o usuário desconectado

                // Notifica todos os outros clientes sobre a desconexão
                await Clients.All.SendAsync("UserDisconnected", userName);
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}