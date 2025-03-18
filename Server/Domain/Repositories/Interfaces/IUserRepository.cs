using chatApp.Server.Domain.Models;

namespace chatApp.Server.Domain.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
