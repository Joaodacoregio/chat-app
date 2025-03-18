using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Models;
using chatApp.Server.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace chatApp.Server.Domain.Repositories.Bases
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IAppDbContext context) : base(context) { }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
