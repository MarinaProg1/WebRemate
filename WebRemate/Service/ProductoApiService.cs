
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
    }



    //public async Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate)
    //{
    //    AgregarTokenAutenticacion();
    //    var response = await _httpClient.GetAsync($"{_apiUrl}/Producto/por-remate/{idRemate}");
    //    if (response.IsSuccessStatusCode)
    //    {
    //        var json = await response.Content.ReadAsStringAsync();
    //        var productos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    //        if (productos != null)
    //        {
    //            foreach (var producto in productos)
    //            {
    //                if (!string.IsNullOrEmpty(producto.Imagen))
    //                {
    //                    // Construir la URL completa de la imagen
    //                    producto.Imagen = $"{_apiUrl}/Producto/imagen/{Path.GetFileName(producto.Imagen)}";
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Imagen no encontrada o nula para el producto: " + producto.IdProducto);
    //                    // Opcional: Asignar una imagen predeterminada si no hay imagen
    //                    producto.Imagen = "/imagenes/LOGO.png"; // Asegúrate de que esta ruta sea correcta
    //                }
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine("Error al deserializar la respuesta JSON.");
    //        }

    //        return productos ?? new List<ProductoViewModel>();
    //    }
    //    else
    //    {
    //        Console.WriteLine($"Error en la API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}"); // Imprime el error de la API
    //    }
    //    return new List<ProductoViewModel>();
    //}
    public async Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate)
    {
        AgregarTokenAutenticacion();
        var response = await _httpClient.GetAsync($"{_apiUrl}/Producto/por-remate/{idRemate}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var productos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (productos != null)
            {
                foreach (var producto in productos)
                {
                    // Verifica si la URL de la imagen no está vacía
                    if (!string.IsNullOrEmpty(producto.Imagenes))
                    {
                     // No es necesario modificar la URL, ya viene completa desde la API
                    }
                    else
                    {
                        Console.WriteLine("Imagen no encontrada o nula para el producto: " + producto.IdProducto);
                        // Opcional: Asignar una imagen predeterminada si no hay imagen
                        producto.Imagenes = "/imagenes/LOGO.png"; // Asegúrate de que esta ruta sea correcta
                    }
                }
            }
            else
            {
                Console.WriteLine("Error al deserializar la respuesta JSON.");
            }

            return productos ?? new List<ProductoViewModel>();
        }
        else
        {
            Console.WriteLine($"Error en la API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}"); // Imprime el error de la API
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
            formData.Add(streamContent, "Imagen", producto.Imagenes.FileName);
        }

        var response = await _httpClient.PostAsync($"{_apiUrl}/Producto/publicar", formData);
        return response.IsSuccessStatusCode;
    }

}

