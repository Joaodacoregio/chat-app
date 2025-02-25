using Microsoft.EntityFrameworkCore;
using chatApp.Server.Models;

namespace chatApp.Server.Data
{
    public class AppDbContext : DbContext
    {
        // Definição da tabela Users (DbSet)
        public DbSet<User> Users { get; set; }

        // Construtor que recebe as opções de configuração
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Configurações adicionais, se necessário
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=chatApp.db"); // Definindo o banco de dados SQLite
            }
        }
    }
}
