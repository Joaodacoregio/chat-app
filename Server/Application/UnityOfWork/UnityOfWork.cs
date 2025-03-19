using chatApp.Server.Application.Bases;
using chatApp.Server.Application.Repositories;
using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Domain.Interfaces.Repository;
using chatApp.Server.Domain.Interfaces.UoW;
 

namespace chatApp.Server.Infrastructure.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        public AppDbContext Context { get; }

        private IUserRepository? _users;
        private IMessageRepository? _messages;
        private IRoomRepository? _rooms;

        //Lazy loading (Para evitar injetar no construtor)
        public IUserRepository Users => _users ??= new UserRepository(Context);
        public IMessageRepository Messages => _messages ??= new MessageRepository(Context);
        public IRoomRepository Rooms => _rooms ??= new RoomRepository(Context);

        public UnitOfWork(AppDbContext context)
        {
            Context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
