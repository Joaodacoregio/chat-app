using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using chatApp.Client;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using chatApp.CookieAuthentication;
using chatApp.Client.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

 
// Configuração do HttpClient para permitir envio de cookies
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://192.168.1.151:5000/") //Porta para requisições do back-end

    };

    // Garantir que os cookies sejam enviados nas requisições
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // Adicionando uma configuração para enviar cookies com credenciais
    client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");

    return client;
});

//Adiciona o serviço de autenticação
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddScoped<RoomService>(); //Isso diz que quando alguém pedir por uma instancia de RoomService, ele vai injetar 

builder.Services.AddAuthorizationCore();

 

await builder.Build().RunAsync();
