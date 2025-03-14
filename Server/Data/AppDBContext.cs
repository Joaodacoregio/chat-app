using Microsoft.EntityFrameworkCore;
using chatApp.Server.Models;
using chatApp.Server.Models.message;

namespace chatApp.Server.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }

}
