using chatApp.Server.Domain.Models;

namespace chatApp.Server.Domain.Interfaces.Bases
{
    //Essa é personalizada para user
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
