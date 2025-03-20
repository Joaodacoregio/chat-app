using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Interfaces.Repository;
using chatApp.Server.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace chatApp.Server.Application.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IAppDbContext _context;
        private readonly DbSet<Message> _dbSet;

        public MessageRepository(IAppDbContext context)
        {
            _context = context;
            _dbSet = ((DbContext)context).Set<Message>();
        }

        public async Task AddMessageAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await _dbSet.AddAsync(message);
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _dbSet.OrderBy(m => m.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByRoomAsync(int RoomId)
        {
            return await _dbSet.Where(m => m.Room.RoomId == RoomId).OrderBy(m => m.Timestamp).ToListAsync();
        }


    }
}
