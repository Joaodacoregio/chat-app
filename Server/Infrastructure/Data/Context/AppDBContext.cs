using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Interfaces.Bases;

namespace chatApp.Server.Data.Context
{
    public class AppDbContext : IdentityDbContext<User>, IAppDbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RoomUser> RoomsUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Chama o método base para configurar as tabelas do Identity

            // Configurar a chave primária composta de RoomUser
            modelBuilder.Entity<RoomUser>()
                .HasKey(ru => new { ru.RoomId, ru.UserId });

            // Configurar relacionamento Room -> RoomUser
            modelBuilder.Entity<RoomUser>()
                .HasOne(ru => ru.Room)
                .WithMany(r => r.RoomUsers)
                .HasForeignKey(ru => ru.RoomId);

            // Configurar relacionamento User -> RoomUser
            modelBuilder.Entity<RoomUser>()
                .HasOne(ru => ru.User)
                .WithMany(u => u.RoomUsers)
                .HasForeignKey(ru => ru.UserId);

            // Criar a sala "Todos" como dado de semente
            modelBuilder.Entity<Room>()
                .HasData(new Room
                {
                    RoomId = 1, // ID fixo
                    Name = "All",
                    Password = null, // Sala pública, sem senha
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Data fixa
                });
        }

        // Implementação explícita dos métodos da interface
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}