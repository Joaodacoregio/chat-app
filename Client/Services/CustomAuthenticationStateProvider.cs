using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;


//Essa classe ela é responsavel por mandar o Cookie contendo o token para o servidor em paginas que precisam de autenticação
public class CookieAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    public CookieAuthStateProvider(HttpClient httpClient, NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentPath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

        // Ignorar verificação em páginas públicas explicitamente //TODO:CRIAR UM MAP OU FACTORY
        if (currentPath.Equals("login", StringComparison.OrdinalIgnoreCase) ||
            currentPath.Equals("register", StringComparison.OrdinalIgnoreCase))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Função que eu fiz em JS que é importada pelo jsRuntime
            var authToken = await _jsRuntime.InvokeAsync<string>("getCookie", "authToken");

            //Verifica se existe
            if (string.IsNullOrEmpty(authToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Adiciona e envia
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var response = await _httpClient.GetAsync("api/auth/check-auth");

            if (response.IsSuccessStatusCode)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Usuário Autenticado")  
                };
                var identity = new ClaimsIdentity(claims, "Bearer");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return new AuthenticationState(claimsPrincipal);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar autenticação: {ex.Message}");
        }

        // Caso o token não seja válido ou tenha ocorrido um erro, retornamos uma identidade não autenticada
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
