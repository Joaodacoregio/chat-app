using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using chatApp.Client;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuração do HttpClient para permitir envio de cookies
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5026")
    };

    // Garantir que os cookies sejam enviados nas requisições
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // Adicionando uma configuração para enviar cookies com credenciais
    client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");

    return client;
});

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddAuthorizationCore(); // Para habilitar a autorização no Blazor

await builder.Build().RunAsync();
