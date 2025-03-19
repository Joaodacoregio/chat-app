using chatApp.Server.Domain.Interfaces.Repository;
using chatApp.Server.Domain.Interfaces.Bases;

namespace chatApp.Server.Domain.Interfaces.UoW
{
    //gerenciar múltiplos repositórios e transações de forma centralizada. 
    //evita multiplas estancias de dbContext e garante que todas as operações sejam executadas em uma única transação.
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IMessageRepository Messages { get; }
        IRoomRepository Rooms { get; }

        Task<int> SaveChangesAsync();
    }
}
