using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace chatApp.Server.Models
{
    public class User
    {
 
        public User()
        {
            Messages = new Collection<Message>();
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

        //img não é obrigatorio e pode ser nula (?)
        [Url]
        public string? Img { get; set; }  

        // Dados offlabel para info do usuario
        public bool IsEmailConfirmed { get; set; } // Indica se o email foi confirmado (opcional)

        [StringLength(500)] // Tamanho máximo do token de recuperação de senha
        public string? PasswordResetToken { get; set; } // Token gerado para recuperação de senha (opcional)

        public DateTime? PasswordResetTokenExpiration { get; set; } // Expiração do token de recuperação (opcional)

        public DateTime LastLogin { get; set; } // Data e hora do último login do usuário (opcional)

        // Definindo IsActive como true por padrão
        public bool IsActive { get; set; } = true; // Indica se a conta está ativa (padrão `true`)

        // Campos de data, podem ser preenchidos automaticamente ou no código
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data de criação da conta (preenchido automaticamente com a data atual)

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Data de última atualização da conta (preenchido automaticamente com a data atual)

        // Adicionar roles/permissões
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User"; // Todos são usuarios para garantir segurança 
        //ADM vai ser definido apenas fornçando comando na DB

        public ICollection<Message> Messages { get; set; } // Relacionamento 1:N com a tabela de mensagens
    }
}
