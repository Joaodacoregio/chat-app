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
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>(); // ConnectionId -> UserName
        private static Dictionary<string, List<string>> _roomConnections = new Dictionary<string, List<string>>(); // RoomId (string) -> Lista de ConnectionIds

        public ChatHub(IUnitOfWork unityOfWork, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _uow = unityOfWork;
            _mapper = mapper;
        }

        public async Task SendMessage(string message, string token, string roomName)
        {
            // Decodificar o token para obter informações do usuário
            var user = _tokenService.DecodeJwtToken(token);
            var userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            Console.WriteLine($"Usuário {userName} enviou a mensagem: {message}");

            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) userId = -1;

            // Validação e obtenção do RoomId a partir do RoomName
            int roomId = await GetRoomIdFromRoomName(roomName);
            if (roomId == -1)
            {
                throw new HubException($"Sala '{roomName}' não encontrada ou inválida.");
            }

            // Criar a nova mensagem com o RoomId obtido
            var newMessage = new Message
            {
                UserId = userId,
                RoomId = roomId,
                UserName = userName,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            // Adicionar a mensagem ao repositório e salvar
            await _uow.Messages.AddMessageAsync(newMessage);
            await _uow.SaveChangesAsync();

            // Mapear a mensagem para DTO usando AutoMapper
            var messageDto = _mapper.Map<MessageDto>(newMessage);

            // Serializar os dados da mensagem para enviar via SignalR
            var messageData = JsonConvert.SerializeObject(messageDto);

            // Enviar a mensagem apenas para os clientes no grupo da sala (usando RoomId como string)
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", messageData);
        }

        public async Task UserConnected(string token, string roomName)
        {
            // Decodificar o token para obter informações do usuário
            var user = _tokenService.DecodeJwtToken(token);
            string userName = user?.Identity?.Name ?? "Usuário Desconhecido";

            // Associar ConnectionId ao usuário
            _userConnections[Context.ConnectionId] = userName;

            // Validação e obtenção do RoomId a partir do RoomName
            int roomId = await GetRoomIdFromRoomName(roomName);
            if (roomId == -1)
            {
                throw new HubException($"Sala '{roomName}' não encontrada ou inválida.");
            }

            string roomIdStr = roomId.ToString();

            // Adicionar o usuário ao grupo da sala
            await Groups.AddToGroupAsync(Context.ConnectionId, roomIdStr);

            // Adicionar ConnectionId à lista de conexões da sala
            if (!_roomConnections.ContainsKey(roomIdStr))
            {
                _roomConnections[roomIdStr] = new List<string>();
            }
            _roomConnections[roomIdStr].Add(Context.ConnectionId);

            // Mapear o usuário para DTO usando AutoMapper
            var userDto = _mapper.Map<User>(new User { Nickname = userName });
            var userConnectedData = JsonConvert.SerializeObject(userDto);

            // Notificar apenas os usuários da sala sobre a nova conexão
            await Clients.Group(roomIdStr).SendAsync("UserConnected", userConnectedData);

            // Enviar o nome do usuário atual apenas para o cliente que conectou
            await Clients.Caller.SendAsync("SetCurrentUser", userName);

            // Enviar a lista de usuários conectados na sala apenas para o cliente que conectou
            var connectedUsers = _roomConnections[roomIdStr].Select(cid => _userConnections[cid]).ToList();
            var connectedUsersJson = JsonConvert.SerializeObject(connectedUsers);
            await Clients.Caller.SendAsync("ConnectedUsers", connectedUsersJson);

            // Carregar histórico usando repository
            var pastMessages = await _uow.Messages.GetMessagesByRoomAsync(roomId); // Ajustado para GetMessagesByRoomAsync
            var pastMessagesDto = _mapper.Map<IEnumerable<MessageDto>>(pastMessages);
            var pastMessagesJson = JsonConvert.SerializeObject(pastMessagesDto);

            // Enviar histórico apenas para o cliente que conectou
            await Clients.Caller.SendAsync("ReceivePastMessages", pastMessagesJson);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                var userName = _userConnections[Context.ConnectionId];

                // Remover o usuário de todas as salas em que estava
                foreach (var room in _roomConnections)
                {
                    if (room.Value.Contains(Context.ConnectionId))
                    {
                        room.Value.Remove(Context.ConnectionId);
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Key);
                        var userDisconnectedData = JsonConvert.SerializeObject(new { userName });
                        await Clients.Group(room.Key).SendAsync("UserDisconnected", userDisconnectedData);
                    }
                }

                // Remover a conexão do dicionário de usuários
                _userConnections.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Método auxiliar para buscar e validar o RoomId com base no RoomName
        private async Task<int> GetRoomIdFromRoomName(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
            {
                return -1; // RoomName inválido
            }

            var room = await _uow.Rooms.GetRoomByNameAsync(roomName);
            if (room == null)
            {
                return -1; // Sala não encontrada
            }

            return room.RoomId; // Retorna o RoomId correspondente
        }
    }
}