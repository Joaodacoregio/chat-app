namespace chatApp.Server.Domain.Interfaces.Services
{

    //Aberta para extensão e fechada para modificação
    public interface ITokenKeeper
        {
            void Save(string token, HttpResponse response);  
        }


}
