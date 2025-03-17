using System.Net.Http.Json;
using static chatApp.Client.Pages.Loby;

namespace chatApp.Client.Service
{
    //Usado apenas para CRUD das salas
    public class RoomCRUD
    {
        private readonly HttpClient _httpClient;

        public RoomCRUD(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Criar sala (C) 
        public async Task<bool> CreateRoomAsync(string roomName, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/room/create", new { roomName, password });
            return response.IsSuccessStatusCode;
        }


        // Buscar salas (R)
        public async Task<List<string>> GetRooms()
        {
            try
            {
                var resposta = await _httpClient.GetAsync("api/room");
                resposta.EnsureSuccessStatusCode();

                var conteudoResposta = await resposta.Content.ReadAsStringAsync();
                var salas = System.Text.Json.JsonSerializer.Deserialize<List<RoomData>>(conteudoResposta);

                return salas?.Select(s => s.roomName).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>();
            }
        }
    }
}
 