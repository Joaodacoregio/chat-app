namespace chatApp.Server.Application.DTOs
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }
}
