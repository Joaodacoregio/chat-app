namespace chatApp.Server.Domain.Interfaces.Services
{
    //Serve para manipular o hub
    public interface IUserConnectionHubService
    {
        void AddUserConnection(string connectionId, string userName);
        void RemoveUserConnection(string connectionId);
        List<string> GetAllUsers();

    }
}


 