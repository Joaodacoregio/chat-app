using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;

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

        // Ignorar verificação em páginas públicas
        if (currentPath.Equals("login", StringComparison.OrdinalIgnoreCase) ||
            currentPath.Equals("register", StringComparison.OrdinalIgnoreCase))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Lê o cookie contendo o token JWT
            var authToken = await _jsRuntime.InvokeAsync<string>("getCookie", "authToken");

            // Se o token não for encontrado, o usuário não está autenticado
            if (string.IsNullOrEmpty(authToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Adiciona o token ao cabeçalho Authorization da requisição HTTP
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Chama o endpoint do back-end para verificar o token
            var response = await _httpClient.GetAsync("api/auth/check-auth");

            if (response.IsSuccessStatusCode)
            {
                // Se o token for válido, retornamos um ClaimsPrincipal com identidade autenticada
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Usuário Autenticado") // Um valor genérico, pois não estamos retornando dados do usuário
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
