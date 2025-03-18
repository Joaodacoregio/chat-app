using chatApp.Server.Domain.Models;
using Microsoft.EntityFrameworkCore;
 

//Essa classe é desnecessaria , eu fiz ela apenas para brincar com interface
namespace chatApp.Server.Data.Context
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Message> Messages { get; }

        DbSet<Room> Rooms { get; }

        DbSet<RoomUser> RoomsUsers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}