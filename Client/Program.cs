using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using chatApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//È para aqui que ele vai mandar os requests esse é o link do servidor.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5026/") });


await builder.Build().RunAsync();
