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

 
// Configura��o do HttpClient para permitir envio de cookies
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://192.168.1.151:5000/") //Porta para requisi��es do back-end

    };

    // Garantir que os cookies sejam enviados nas requisi��es
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // Adicionando uma configura��o para enviar cookies com credenciais
    client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");

    return client;
});

//Adiciona o servi�o de autentica��o
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddScoped<RoomService>(); //Isso diz que quando algu�m pedir por uma instancia de RoomService, ele vai injetar 

builder.Services.AddAuthorizationCore();

 

await builder.Build().RunAsync();
