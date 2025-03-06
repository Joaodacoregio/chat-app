using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Models.requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Nickname { get; set; } = string.Empty;

        [Url]
        public string? Img { get; set; } // Pode ser nula

    }
}
