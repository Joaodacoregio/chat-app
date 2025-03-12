using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using chatApp.Client;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

 


// Configura��o do HttpClient para permitir envio de cookies
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://192.168.1.162:5000/") // Backend na porta 5000

    };

    // Garantir que os cookies sejam enviados nas requisi��es
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // Adicionando uma configura��o para enviar cookies com credenciais
    client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");

    return client;
});

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddAuthorizationCore();  

await builder.Build().RunAsync();
