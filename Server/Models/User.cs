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
        public string Apelido { get; set; } = string.Empty;

        public byte[]? Imagem { get; set; } // Foto de perfil opcional
    }

}
