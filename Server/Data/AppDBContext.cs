using Microsoft.EntityFrameworkCore;
using chatApp.Server.Models;
using chatApp.Server.Models.message;

namespace chatApp.Server.Data
{
    public class AppDbContext : DbContext
    {

        //TABELAS
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; } // Adicionando a tabela Messages

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=chatApp.db");  
            }
        }
    }
}
