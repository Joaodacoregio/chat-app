using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Models
{
    public class User
    {
        public User()
        {
            Messages = new Collection<Message>();
            RoomUsers = new Collection<RoomUser>();
        }

        [Key]
        public int Id { get; set; }

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
        public string? Img { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
        public ICollection<RoomUser> RoomUsers { get; set; } // Relacionamento N:N via tabela intermediária
    }
}