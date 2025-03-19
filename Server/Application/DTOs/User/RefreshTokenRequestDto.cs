namespace chatApp.Server.Application.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
 