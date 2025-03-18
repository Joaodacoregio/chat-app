using chatApp.Server.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using chatApp.Server.Data.Context;

namespace chatApp.Server.Presentation.Hubs
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

            Console.WriteLine($"Usuário {userName} enviou a mensagem: {message}");
            // Obter o ID do usuário a partir do claim NameIdentifier
            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Tentar converter o userId para int
            if (!int.TryParse(userIdString, out int userId))
            {
                // Se a conversão falhar, podemos definir um valor padrão ou lançar uma exceção
                userId = -1;  // Definir -1 ou qualquer valor que seja considerado inválido
            }

            // Salvar a mensagem no banco de dados
            var newMessage = new Message
            {
                UserId = userId,    
                RoomId = 1, // Isso é um teste , TODO:ARRUMAR ISSO È UM TESTE
                UserName = userName,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            // Adicionar a nova mensagem ao contexto
            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Biblioteca para JSON (Newtonsoft)
            var messageData = JsonConvert.SerializeObject(new
            {
                userName,
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
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "ID Desconhecido";

            var claims = jwtToken.Claims.Concat(new[]
            {
                 new Claim(ClaimTypes.Name, userName),
                 new Claim(ClaimTypes.NameIdentifier, userId)
            });

            // Criando e retornando o ClaimsPrincipal
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
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