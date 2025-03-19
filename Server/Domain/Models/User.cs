using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Domain.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Messages = new Collection<Message>();
            RoomUsers = new Collection<RoomUser>();
        }

        [StringLength(50)]
        public string Nickname { get; set; } = string.Empty;

        [Url]
        public string? Img { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<RoomUser> RoomUsers { get; set; } // Relacionamento N:N via tabela intermediária
    }
}
