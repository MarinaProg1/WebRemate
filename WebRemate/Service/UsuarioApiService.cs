
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using WebRemate.Interfaces;
using WebRemate.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebRemate.Service
{
    public class UsuarioApiService : IUsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiUrl;

        public UsuarioApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apiUrl = configuration["ApiUrl"];
        }

        public async Task<bool> Registrarse(RegistroUsuarioViewModel modelo)
        {
            var json = JsonSerializer.Serialize(modelo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Acceso/Registrarse", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<TokenResponse?> Login(LoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Acceso/Login", content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.token))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Cambiar a false en desarrollo si es necesario
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwt_token", tokenResponse.token, cookieOptions);
            }

            return tokenResponse;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("jwt_token");
        }

        public string? ObtenerToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];
        }
    }
}
