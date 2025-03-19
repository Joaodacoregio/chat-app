using Microsoft.EntityFrameworkCore;
using chatApp.Server.Domain.Models;

namespace chatApp.Server.Data.Context
{
    public interface IAppDbContext : IDisposable
    {
        DbSet<Room> Rooms { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<RoomUser> RoomsUsers { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}