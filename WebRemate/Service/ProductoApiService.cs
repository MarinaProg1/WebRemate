//using Microsoft.AspNetCore.Http;
//using Newtonsoft.Json;
//using System.Net.Http.Headers;
//using WebRemate.Models;
//using WebRemate.Interfaces;
//using System.Net.Http;
//using System.Text;

//namespace WebRemate.Service
//{
//    public class ProductoApiService : IProductoApiService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _baseUrl;

//        public ProductoApiService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _baseUrl = configuration["ApiUrl"];
//        }
//        public async Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate)
//        {

//            var response = await _httpClient.GetAsync($"{_baseUrl}/Producto/por-remate/{idRemate}");

//            if (!response.IsSuccessStatusCode)
//                return new List<ProductoViewModel>();

//            var json = await response.Content.ReadAsStringAsync();
//            return JsonConvert.DeserializeObject<List<ProductoViewModel>>(json);
//        }

//        public async Task<bool> PublicarProducto(ProductoViewModel producto)
//        {
//            var json = JsonConvert.SerializeObject(producto);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");
//            var response = await _httpClient.PostAsync($"{_baseUrl}/producto/publicar", content);

//            return response.IsSuccessStatusCode;
//        }
//    }


//}
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using WebRemate.Interfaces;
using WebRemate.Models;

public class ProductoApiService : IProductoApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductoApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _apiUrl = configuration["ApiUrl"];
        _httpContextAccessor = httpContextAccessor;
    }

    private void AgregarTokenAutenticacion()
    {

        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];
       // var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt_token");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
    }

   


    public async Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate)
    {
        AgregarTokenAutenticacion();

        var response = await _httpClient.GetAsync($"{_apiUrl}/Producto/por-remate/{idRemate}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<List<ProductoViewModel>>();
    }

    //public async Task<bool> PublicarProducto(ProductoViewModel producto)
    //{
    //    AgregarTokenAutenticacion();

    //    var json = JsonSerializer.Serialize(producto);
    //    var content = new StringContent(json, Encoding.UTF8, "application/json");

    //    var response = await _httpClient.PostAsync($"{_apiUrl}/Producto/publicar", content);
    //    return response.IsSuccessStatusCode;
    //}

    public async Task<bool> PublicarProducto(ProductoViewModel producto)
    {
        AgregarTokenAutenticacion();

        using var formData = new MultipartFormDataContent();

        // Agregar los datos del producto
        formData.Add(new StringContent(producto.Titulo), "Titulo");
        formData.Add(new StringContent(producto.Descripcion), "Descripcion");
        formData.Add(new StringContent(producto.PrecioBase.ToString()), "PrecioBase");
        formData.Add(new StringContent(producto.IdRemate.ToString()), "IdRemate");
        formData.Add(new StringContent(producto.IdUsuario.ToString()), "IdUsuario");

        // Agregar la imagen si existe
        if (producto.Imagen != null && producto.Imagen.Length > 0)
        {
            var streamContent = new StreamContent(producto.Imagen.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(producto.Imagen.ContentType);
            formData.Add(streamContent, "Imagen", producto.Imagen.FileName);
        }

        var response = await _httpClient.PostAsync($"{_apiUrl}/Producto/publicar", formData);
        return response.IsSuccessStatusCode;
    }

}

