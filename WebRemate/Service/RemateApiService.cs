using Newtonsoft.Json;
using WebRemate.Models;
using WebRemate.Interfaces;

namespace WebRemate.Service
{
    public class RemateApiService: IRemateApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        
        public RemateApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiUrl"];
        }

        // Obtener todas las subastas activas
        public async Task<List<RemateViewModels>> ObtenerSubastasActivas()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/remates/activas");

            if (!response.IsSuccessStatusCode)
                return new List<RemateViewModels>();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RemateViewModels>>(responseBody) ?? new List<RemateViewModels>();
        }

        // Obtener todos los remates
        public async Task<List<RemateViewModels>> ObtenerTodosLosRemates()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/remates/todos");

            if (!response.IsSuccessStatusCode)
                return new List<RemateViewModels>();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RemateViewModels>>(responseBody) ?? new List<RemateViewModels>();
        }
    }
}
