using Microsoft.AspNetCore.Mvc;
using WebRemate.Interfaces;
using WebRemate.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using System.IO;
using WebRemate.Service;

[Authorize]
public class ProductoController : Controller
{
    private readonly IProductoApiService _productoApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly IRemateApiService _remateApiService; 
    public ProductoController(IProductoApiService productoApiService, IHttpContextAccessor httpContextAccessor, IRemateApiService remateApiService)
    {
        _productoApiService = productoApiService;
        _httpContextAccessor = httpContextAccessor;
        _remateApiService = remateApiService;   
    }



    [HttpGet]
    public async Task<IActionResult> ProductosPorRemate(int idRemate)
    {
        if (!User.Identity.IsAuthenticated || User.Identity == null)
        {
            return Challenge();
        }

        var productos = await _productoApiService.ObtenerProductosPorRemate(idRemate);

        if (productos == null || !productos.Any())
        {
            ViewBag.ErrorMessage = "No hay productos en este remate.";
            return View("SinProductos"); // Vista específica para cuando no hay productos
        }

        var remate = await _remateApiService.ObtenerRematePorId(idRemate);
        ViewBag.EstadoRemate = remate?.Estado ?? "Desconocido";
        ViewBag.FechaInicio = remate?.FechaInicio.ToString("dd/MM/yyyy") ?? "No especificada";

        return View(productos);
    }


    [HttpGet]
    public IActionResult Publicar(int idRemate)
    {
        // Obtener el token desde la cookie
        var token = Request.Cookies["jwt_token"];

        if (string.IsNullOrEmpty(token))
        {
            return Challenge(); // Redirigir al login si no hay token
        }

        try
        {
            // Decodificar el token y extraer el ID del usuario
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idUsuarioClaim == null)
            {
                return Challenge(); // Redirigir si no se encuentra el claim
            }

            // Convertir el ID del usuario a entero
            if (!int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return Challenge(); // Redirigir si el ID no es válido
            }

            // Crear el modelo con los valores preasignados
            var model = new CrearProductoViewModel
            {
                IdRemate = idRemate,
                IdUsuario = idUsuario
            };

            return View(model); // Pasamos el modelo a la vista
        }
        catch (Exception)
        {
            return Challenge(); // Si hay un error, redirigir al login
        }
    }

    [HttpPost]
    public async Task<IActionResult> Publicar(CrearProductoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Obtener el token desde la cookie
        var token = Request.Cookies["jwt_token"];

        if (string.IsNullOrEmpty(token))
        {
            return Challenge(); // Redirigir al login si no hay token
        }

        // Decodificar el token para obtener el ID del usuario
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
        {
            return Challenge(); // Redirigir si no se encuentra el claim o el ID no es válido
        }

        model.IdUsuario = idUsuario;

        bool publicado = await _productoApiService.PublicarProducto(model);

        if (!publicado)
        {
            ModelState.AddModelError(string.Empty, "Error al publicar el producto.");
            return View(model);
        }

      
        TempData["PublicacionExitosa"] = "¡El producto se publicó exitosamente!";
        ModelState.Clear();  

        return RedirectToAction("Publicar"); 
    }



}
