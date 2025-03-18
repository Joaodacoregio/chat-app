using chatApp.Server.Domain.Models;

namespace chatApp.Server.Domain.Interfaces.Bases
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
