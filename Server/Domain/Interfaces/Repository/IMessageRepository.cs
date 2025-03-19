using chatApp.Server.Domain.Models;

namespace chatApp.Server.Domain.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetAllMessagesAsync();
    }
}
