namespace chatApp.Server.Application.DTOs 
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string? Img { get; set; }
    }
}
