using chatApp.Server.Domain.Models;

namespace chatApp.Server.Domain.Interfaces.Bases
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<Room?> GetRoomByNameAsync(string roomName);
    }
}
