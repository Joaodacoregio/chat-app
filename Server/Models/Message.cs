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
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public int UserId { get; set; } // Chave estrangeira para User

        [ForeignKey("RoomId")]
        public int RoomId { get; set; } // Chave estrangeira para Room

        public virtual User User { get; set; } = default!;
        public virtual Room Room { get; set; } = default!;
    }
}