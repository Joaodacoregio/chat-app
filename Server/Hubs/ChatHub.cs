using chatApp.Server.Data;
using chatApp.Server.Models;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;

namespace chatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IAppDbContext _dbContext;
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();

        public ChatHub(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendMessage(string message, string token)
        {
            var user = DecodeJwtToken(token);
            var userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            // Salvar a mensagem no banco de dados
            var newMessage = new Message
            {
                UserName = userName,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Biblioteca para JSON Newtonsoft
            var messageData = JsonConvert.SerializeObject(new
            {
                userName = userName,
                content = message,
                timestamp = newMessage.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            });
            // Enviar a mensagem como JSON para todos os clientes conectados
            await Clients.All.SendAsync("ReceiveMessage", messageData);
        }

        private ClaimsPrincipal DecodeJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Usuário Desconhecido";
            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
        }

        public async Task UserConnected(string token)
        {
            var user = DecodeJwtToken(token);
            string userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            _userConnections[Context.ConnectionId] = userName;

            // Enviar JSON com o nome do usuário conectado
            var userConnectedData = JsonConvert.SerializeObject(new { userName });
            await Clients.All.SendAsync("UserConnected", userConnectedData);

            // Enviar o nome do usuário atual ao cliente
            await Clients.Caller.SendAsync("SetCurrentUser", userName);

            // Enviar lista de usuários conectados como JSON
            var connectedUsers = JsonConvert.SerializeObject(_userConnections.Values.ToList());
            await Clients.Caller.SendAsync("ConnectedUsers", connectedUsers);

            // Enviar histórico de mensagens como JSON
            var pastMessages = _dbContext.Messages
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    userName = m.UserName,
                    content = m.Content,
                    timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();

            var pastMessagesJson = JsonConvert.SerializeObject(pastMessages);
            await Clients.Caller.SendAsync("ReceivePastMessages", pastMessagesJson);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                var userName = _userConnections[Context.ConnectionId];
                _userConnections.Remove(Context.ConnectionId);

                // Enviar JSON com o nome do usuário desconectado
                var userDisconnectedData = JsonConvert.SerializeObject(new { userName });
                await Clients.All.SendAsync("UserDisconnected", userDisconnectedData);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}