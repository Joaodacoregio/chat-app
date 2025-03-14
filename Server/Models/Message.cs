using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chatApp.Server.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey("UserId")] // Chave estrangeira para o usuário que enviou a mensagem
        public int UserId { get; set; } 

    }
}