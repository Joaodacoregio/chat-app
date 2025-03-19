using AutoMapper;
using chatApp.Server.Application.DTOs;
using chatApp.Server.Domain.Models;
using chatApp.Server.Services.Interfaces;
using chatApp.Server.Domain.Interfaces.UoW;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Newtonsoft.Json;

namespace chatApp.Server.Presentation.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();

        public ChatHub(IUnitOfWork unityOfWork, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _uow = unityOfWork;
            _mapper = mapper;
        }

        public async Task SendMessage(string message, string token)
        {
            var user = _tokenService.DecodeJwtToken(token);
            var userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            Console.WriteLine($"Usuário {userName} enviou a mensagem: {message}");

            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) userId = -1;

            var newMessage = new Message
            {
                UserId = userId,
                RoomId = 1, // TODO: Ajustar futuramente
                UserName = userName,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await _uow.Messages.AddMessageAsync(newMessage);
            await _uow.SaveChangesAsync();

            // Mapear a mensagem para DTO usando AutoMapper
            var messageDto = _mapper.Map<MessageDto>(newMessage);

            // Serializando os dados da mensagem para enviar via SignalR
            var messageData = JsonConvert.SerializeObject(messageDto);

            await Clients.All.SendAsync("ReceiveMessage", messageData);
        }

        public async Task UserConnected(string token)
        {
            var user = _tokenService.DecodeJwtToken(token);
            string userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            _userConnections[Context.ConnectionId] = userName;

            // Mapear o usuário para DTO usando AutoMapper
            var userDto = _mapper.Map<User>(new User { Nickname = userName });

            var userConnectedData = JsonConvert.SerializeObject(userDto);
            await Clients.All.SendAsync("UserConnected", userConnectedData);
            await Clients.Caller.SendAsync("SetCurrentUser", userName);

            var connectedUsers = JsonConvert.SerializeObject(_userConnections.Values.ToList());
            await Clients.Caller.SendAsync("ConnectedUsers", connectedUsers);

            // Carregar histórico usando repository
            var pastMessages = await _uow.Messages.GetAllMessagesAsync();
            var pastMessagesDto = _mapper.Map<IEnumerable<MessageDto>>(pastMessages);

            var pastMessagesJson = JsonConvert.SerializeObject(pastMessagesDto);

            await Clients.Caller.SendAsync("ReceivePastMessages", pastMessagesJson);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                var userName = _userConnections[Context.ConnectionId];
                _userConnections.Remove(Context.ConnectionId);

                var userDisconnectedData = JsonConvert.SerializeObject(new { userName });
                await Clients.All.SendAsync("UserDisconnected", userDisconnectedData);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
