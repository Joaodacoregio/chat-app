 
using System.Net.Http.Json;

 
namespace chatApp.Client.Service
{
    //Classe responsavel pela validação 
    public class RoomValidator
    {
        private readonly HttpClient _httpClient;

        public RoomValidator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsRoomNameValid(string roomName)
        {
            return !string.IsNullOrEmpty(roomName) && roomName.Length >= 3;
        }

        public async Task<bool> IsPublicRoomAsync(string roomName)
        {
            var response = await _httpClient.PostAsJsonAsync("api/room/publicroom", new { roomName });

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false; // Retorna false se a requisição falhar
        }
    }

}