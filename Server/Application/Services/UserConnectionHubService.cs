using chatApp.Server.Domain.Interfaces.Services;

public class UserConnectionHubService : IUserConnectionHubService
{
    private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();

    public void AddUserConnection(string connectionId, string userName)
    {
        _userConnections[connectionId] = userName;
    }

    public void RemoveUserConnection(string connectionId)
    {
        if (_userConnections.ContainsKey(connectionId))
            _userConnections.Remove(connectionId);
    }

    public List<string> GetAllUsers() => _userConnections.Values.ToList();
}