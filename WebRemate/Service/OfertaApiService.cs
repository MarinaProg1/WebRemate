using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebRemate.Interfaces;
using WebRemate.Models;

namespace WebRemate.Service
{
    public class OfertaApiService: IOfertaApiService
    {


        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OfertaApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiUrl"];
            _httpContextAccessor = httpContextAccessor;
        }

        private void AgregarTokenAutenticacion()
        {

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<(bool Exito, string Mensaje)> RealizarOferta(OfertaViewModel model)
       
        {
            AgregarTokenAutenticacion();

            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/oferta/ofertar", content);


            if (response.IsSuccessStatusCode)
            {
                return (true, "Oferta realizada exitosamente.");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage);
        }

        public async Task<List<MostrarOfertaViewModel>> ObtenerOfertas(int idProducto)
        {
            AgregarTokenAutenticacion();

            var response = await _httpClient.GetAsync($"{_apiUrl}/oferta/ofertas/{idProducto}");
            if (!response.IsSuccessStatusCode) return new List<MostrarOfertaViewModel>();

            var ofertas = await response.Content.ReadFromJsonAsync < List <MostrarOfertaViewModel>>();
            return ofertas;
        }

        public async Task<(bool Exito, string Mensaje, FacturaViewModel? Factura)> SeleccionarOfertaGanadora(int idProducto)
        {
            AgregarTokenAutenticacion();

            var response = await _httpClient.PostAsync($"{_apiUrl}/oferta/seleccionar-ganadora/{idProducto}", null);

            if (response.IsSuccessStatusCode)
            {
                var factura = await response.Content.ReadFromJsonAsync<FacturaViewModel>();
                return (true, "Oferta ganadora seleccionada y factura generada.", factura);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage, null);
        }


    }
}
