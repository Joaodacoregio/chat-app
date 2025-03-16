using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace chatApp.Server.Models
{
    public class Room
    {
        public Room()
        {
            Messages = new Collection<Message>();
            RoomUsers = new Collection<RoomUser>();
        }

        [Key]
        public int RoomId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Password { get; set; } // Senha da sala (opcional)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
        public ICollection<RoomUser> RoomUsers { get; set; } // Relacionamento N:N via tabela intermediária
    }
}