using System.Net.Http.Json;

namespace chatApp.Client.Service
{
    //Serve como uma orquestra que integra o sistema de salas
    public class RoomManager
    {
        private readonly RoomCRUD _roomCRUD;
        private readonly RoomValidator _roomValidator;
        private readonly HttpClient _httpClient;

        public RoomManager(RoomCRUD roomCrud, RoomValidator roomValidator , HttpClient httpClient)
        {
            _roomCRUD = roomCrud ?? throw new ArgumentNullException(nameof(roomCrud));
            _roomValidator = roomValidator;
            _httpClient = httpClient;

        }

        // Criar sala
        public async Task<bool> CreateRoomAsync(string roomName, string password)
        {
            // Uso a classe RoomValidator para validar os dados
            if (!_roomValidator.IsRoomNameValid(roomName))
            {
                return false;
            }

            // Uso a classe RoomCRUD para criar a sala
            var isCreated = await _roomCRUD.CreateRoomAsync(roomName, password);
            return isCreated;
        }

        // Validar sala
        public async Task<bool> ValidateRoomPasswordAsync(string roomName, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/room/validate", new { roomName, password });
            return response.IsSuccessStatusCode;
        }

        // Buscar salas
        public async Task<List<string>> GetAvailableRoomsAsync()
        {
            return await _roomCRUD.GetRooms();
        }

        public async Task<bool> IsPublicRoomAsync(string roomName)
        {
            return await _roomValidator.IsPublicRoomAsync(roomName);
        }

    }
}
