using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
 
    

namespace chatApp.Client.Service
{
    public class RoomService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoomService> _logger;

        public RoomService(HttpClient httpClient, ILogger<RoomService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

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
                _logger.LogError(ex, "Erro ao carregar salas disponíveis");
                return new List<string>();
            }
        }
    }

    public class RoomData
    {
        public int roomId { get; set; } // Agora roomId é int
        public string roomName { get; set; } = string.Empty;
        public string createdAt { get; set; } = string.Empty;
    }
}