using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;
using static chatApp.Client.Pages.Login;
using System.Net.Http.Json;


namespace chatApp.CookieAuthentication
{
    //Essa classe ela é responsavel por mandar o Cookie contendo o token para o servidor em paginas que precisam de autenticação
    public class CookieAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        //To usando para fazer a manipução de cookie , TODO: CRIAR MANIPULAÇÃO POR C#
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
                var accessToken = await _jsRuntime.InvokeAsync<string>("getCookie", "accessToken");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Verifica se o access token é válido
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Token expirado, tenta renovar com o refresh token
                    var refreshToken = await _jsRuntime.InvokeAsync<string>("getCookie", "refreshToken");

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var refreshRequest = new RefreshTokenRequest
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        };

                        var refreshResponse = await _httpClient.PostAsJsonAsync("api/auth/refresh", refreshRequest);

                        if (refreshResponse.IsSuccessStatusCode)
                        {
                            var authResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
                            var newAccessToken = authResponse?.Token;
                            var newRefreshToken = authResponse?.RefreshToken;

                            if (!string.IsNullOrEmpty(newAccessToken))
                            {
                                // Atualiza os tokens nos cookies
                                await _jsRuntime.InvokeVoidAsync("setCookie", "accessToken", newAccessToken, 1);
                                if (!string.IsNullOrEmpty(newRefreshToken))
                                {
                                    await _jsRuntime.InvokeVoidAsync("setCookie", "refreshToken", newRefreshToken, 7);
                                }

                                // Tenta novamente a autenticação com o novo token
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                                return await GetAuthenticationStateAsync();
                            }
                        }
                    }
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

}
