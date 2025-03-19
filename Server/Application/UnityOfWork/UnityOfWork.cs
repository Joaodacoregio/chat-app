using chatApp.Server.Application.Bases;
using chatApp.Server.Application.Repositories;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Domain.Interfaces.Repository;
using chatApp.Server.Domain.Interfaces.UoW;
using chatApp.Server.Data.Context;

namespace chatApp.Server.Application.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAppDbContext Context { get; }

        private IUserRepository? _users;
        private IMessageRepository? _messages;
        private IRoomRepository? _rooms;

        public IUserRepository Users => _users ??= new UserRepository(Context);
        public IMessageRepository Messages => _messages ??= new MessageRepository(Context);
        public IRoomRepository Rooms => _rooms ??= new RoomRepository(Context);

        public UnitOfWork(IAppDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
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