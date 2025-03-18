namespace chatApp.Server.Domain.Interfaces.Services
{
    //Da para salvar o token por cookie , localstorage e session storage 
    //Essa interface vai implementar qualquer tipo de metodo para salvar o token
        public interface ITokenKeeper
        {
            void Save(string token, HttpResponse response);  
        }


}
