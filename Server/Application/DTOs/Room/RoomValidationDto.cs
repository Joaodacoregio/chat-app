namespace chatApp.Server.Application.DTOs
{
    public class RoomValidationDto
    {
        public string RoomName { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}
