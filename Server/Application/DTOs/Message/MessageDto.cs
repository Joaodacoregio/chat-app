namespace chatApp.Server.Application.DTOs
{
    public class MessageDto
    {
        //Deixa minuscula pois o JSON vai se basear nesses nomes para o client

        public string? userName { get; set; }
        public string? content { get; set; }
        public string? timestamp { get; set; }
    }
}
