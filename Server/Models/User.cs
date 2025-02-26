using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Nickname { get; set; } = string.Empty;

        public byte[]? Img { get; set; } // Foto de perfil opcional

        [Required]
        public string Password { get; set; } = string.Empty; // Senha obrigatória
    }


}
