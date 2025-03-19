namespace chatApp.Server.Application.DTOs
{
    public class UserLoginResponseDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Nickname { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
