using Newtonsoft.Json;
using WebRemate.Models;
using WebRemate.Interfaces;
using System.Net;

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

        public async Task<List<RemateViewModels>> ObtenerTodosLosRemates()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/remates/todos");

            // Si la respuesta es 404 Not Found, devuelve una lista vacía
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<RemateViewModels>();
            }

            // Si la respuesta es exitosa (200 OK)
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RemateViewModels>>(responseBody) ?? new List<RemateViewModels>();
            }

            // En caso de error, lanza una excepción
            throw new Exception($"Error al obtener los remates: {response.ReasonPhrase}");
        }

        public async Task<RemateViewModels> ObtenerRematePorId(int idRemate)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/remates/{idRemate}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RemateViewModels>(json);
            }

            return null;
        }

        public async Task<GanadorViewModel> CalcularOfertaGanadoraPorProducto(int idProducto)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/remates/calcular-oferta-ganadora/{idProducto}", null);

            // Si la respuesta es exitosa (200 OK)
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GanadorViewModel>(responseBody);
            }

            // Si la respuesta es 404 Not Found
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            // Si hay un error interno en el servidor
            throw new Exception($"Error al calcular la oferta ganadora: {response.ReasonPhrase}");
        }
        public async Task<List<OfertaGanadoraViewModel>> CalcularOfertasGanadoras()
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/remates/calcular-ofertas-ganadoras", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<OfertaGanadoraViewModel>>(jsonString);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<OfertaGanadoraViewModel>(); // No hubo ofertas ganadoras
            }
            else
            {
                throw new Exception("Error al calcular ofertas ganadoras");
            }
        }
    
}
}
