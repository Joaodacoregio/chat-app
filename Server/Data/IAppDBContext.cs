using chatApp.Server.Models.message;
using chatApp.Server.Models;
using Microsoft.EntityFrameworkCore;



namespace chatApp.Server.Data
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Message> Messages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}