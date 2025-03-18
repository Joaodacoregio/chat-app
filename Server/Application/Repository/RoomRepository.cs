using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Interfaces.Bases;
using chatApp.Server.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace chatApp.Server.Application.Bases
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(IAppDbContext context) : base(context) { }

        public async Task<Room?> GetRoomByNameAsync(string roomName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == roomName);
        }
    }
}
