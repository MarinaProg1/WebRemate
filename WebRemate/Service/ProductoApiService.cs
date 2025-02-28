
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using WebRemate.Interfaces;
using WebRemate.Models;
using Newtonsoft.Json;
using System.Text.Json;
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

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate)
    {
        AgregarTokenAutenticacion();
        var response = await _httpClient.GetAsync($"{_apiUrl}/Producto/por-remate/{idRemate}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();

            // Verificar si el JSON contiene una lista o un mensaje
            if (json.Contains("message"))
            {
                var mensaje = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                Console.WriteLine(mensaje["message"]);
                return new List<ProductoViewModel>(); // Retornar lista vacía
            }
            else
            {
                var productos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (productos != null)
                {
                    foreach (var producto in productos)
                    {
                        if (string.IsNullOrEmpty(producto.Imagenes))
                        {
                            producto.Imagenes = "/imagenes/LOGO.png";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error al deserializar la respuesta JSON.");
                }

                return productos ?? new List<ProductoViewModel>();
            }
        }
        else
        {
            Console.WriteLine($"Error en la API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        return new List<ProductoViewModel>();
    }

    public async Task<bool> PublicarProducto(CrearProductoViewModel producto)
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
        if (producto.Imagenes != null && producto.Imagenes.Length > 0)
        {
            var streamContent = new StreamContent(producto.Imagenes.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(producto.Imagenes.ContentType);
            formData.Add(streamContent, "Imagenes", producto.Imagenes.FileName);
        }

        var response = await _httpClient.PostAsync($"{_apiUrl}/Producto/publicar", formData);
        return response.IsSuccessStatusCode;
    }

    

}



